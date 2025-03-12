using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Helpers;

public static class QueryableExtensions
{
    /// <summary>
    /// Filters the query based on a search value across specified properties.
    /// </summary>
    public static IQueryable<T> WhereFilter<T>(
        this IQueryable<T> query,
        List<string>? propertyNames,
        object? value) where T : EntityId
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var properties = GetPropertiesToFilter<T>(propertyNames);
        var filterExpressions = CreateFilterExpressions(parameter, properties, value);
        var combinedExpression = filterExpressions.Aggregate(Expression.OrElse);
        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

        return query.Where(lambda);
    }

    /// <summary>
    /// Filters the query to include only active entities.
    /// </summary>
    public static IQueryable<T> WhereActive<T>(
        this IQueryable<T> query) where T : Entity
    {
        return query.Where(x => x.Active);
    }

    /// <summary>
    /// Filters the query to include the entity with the id.
    /// </summary>
    public static IQueryable<T> WhereId<T>(
        this IQueryable<T> query, long id) where T : EntityId
    {
        return query.Where(x => x.Id == id);
    }

    /// <summary>
    /// Asynchronously counts the number of entities in the database based on optional filtering criteria.
    /// </summary>
    /// <param name="query">A previous query</param>
    /// <param name="queryFilter">Query filter data for sorting, searching, and pagination.</param>
    /// <param name="searchParameters">Additional parameters to filter the count operation.</param>
    /// <returns>A task representing the asynchronous count operation, returning the total count of entities.</returns>
    public static async Task<int> CountAsync<T>(
        this IQueryable<T> query, QueryFilterDto? queryFilter, List<string>? searchParameters) where T : EntityId
    {
        // Aplica el filtro de búsqueda y activo si es necesario
        if (queryFilter != null && !string.IsNullOrEmpty(queryFilter.Search))
        {
            query = query
                .WhereFilter(searchParameters, queryFilter.Search);
        }

        return await query.CountAsync();
    }

    /// <summary>
    /// Applies filtering, sorting, pagination, and returns a paginated list asynchronously.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to apply filters and pagination.</param>
    /// <param name="queryFilter">Filtering criteria including search, sort, and pagination.</param>
    /// <param name="searchParameters">List of property names to apply search filters.</param>
    /// <returns>A paginated list of entities.</returns>
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        QueryFilterDto queryFilter,
        List<string>? searchParameters = null) where T : Entity
    {
        var totalCount = await query
            .WhereActive()
            .CountAsync();

        var items = await query
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .ToListAsync();

        return new PaginatedList<T>(items, totalCount);
    }

    /// <summary>
    /// Applies filtering, sorting, pagination, without checking active state and returns a paginated list asynchronously.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to apply filters and pagination.</param>
    /// <param name="queryFilter">Filtering criteria including search, sort, and pagination.</param>
    /// <param name="searchParameters">List of property names to apply search filters.</param>
    /// <returns>A paginated list of entities.</returns>
    public static async Task<PaginatedList<T>> ToPaginatedListNotActiveAsync<T>(
        this IQueryable<T> query,
        QueryFilterDto queryFilter,
        List<string>? searchParameters = null) where T : EntityId
    {
        var totalCount = await query
            .CountAsync();

        var items = await query
            .ApplyQueryFilter(queryFilter, searchParameters)
            .ToListAsync();

        return new PaginatedList<T>(items, totalCount);
    }

    /// <summary>
    /// Sorts the query by a specified property and direction.
    /// </summary>
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> query,
        string? sortBy,
        string? sortOrder) where T : EntityId
    {
        if (string.IsNullOrWhiteSpace(sortBy) || string.IsNullOrWhiteSpace(sortOrder))
            return query;

        if (!IsValidSortOrder(sortOrder))
            throw new ArgumentException("Sort order must be 'asc' or 'desc'.", nameof(sortOrder));

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyExpression = BuildPropertyExpression(parameter, sortBy);
        var lambda = Expression.Lambda(propertyExpression, parameter);
        var methodName = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

        return query.Provider.CreateQuery<T>(
            Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), propertyExpression.Type],
                query.Expression,
                Expression.Quote(lambda)));
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize) where T : EntityId
    {
        if (pageNumber < 1 || pageSize < 1)
            return query;

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Applies a query filter including search, sorting, and pagination.
    /// </summary>
    public static IQueryable<T> ApplyQueryFilter<T>(
        this IQueryable<T> query,
        QueryFilterDto? filter,
        List<string>? filterProperties = null) where T : EntityId
    {
        if (filter == null)
            return query;

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.WhereFilter(filterProperties, filter.Search);

        if (!string.IsNullOrWhiteSpace(filter.OrderBy) && !string.IsNullOrWhiteSpace(filter.OrderDirection))
            query = query.OrderByDynamic(filter.OrderBy, filter.OrderDirection);

        if (filter.PageNumber > 0 && filter.PageSize > 0)
            query = query.Paginate(filter.PageNumber, filter.PageSize);

        return query;
    }

    /// <summary>
    /// Applies a query filter and restricts to active entities.
    /// </summary>
    public static IQueryable<T> ApplyQueryFilterAndActive<T>(
        this IQueryable<T> query,
        QueryFilterDto? filter,
        List<string>? filterProperties = null) where T : Entity
    {
        return query.WhereActive().ApplyQueryFilter(filter, filterProperties);
    }

    #region Private Helper Methods

    private static List<string> GetPropertiesToFilter<T>(List<string>? propertyNames)
    {
        if (propertyNames?.Count > 0)
            return propertyNames;

        return typeof(T).GetProperties()
            .Where(p => IsFilterableType(p.PropertyType))
            .Select(p => p.Name)
            .ToList();
    }

    private static bool IsFilterableType(Type type)
    {
        return type == typeof(string) ||
               type.IsPrimitive ||
               type == typeof(decimal) ||
               type == typeof(DateTime);
    }

    private static List<Expression> CreateFilterExpressions(
        ParameterExpression parameter,
        List<string> propertyNames,
        object value)
    {
        var filterValue = value.ToString()?.ToLowerInvariant();
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) }) ??
            throw new InvalidOperationException("Contains method not found.");

        return propertyNames.Select(propertyName =>
        {
            var propertyExpr = BuildPropertyExpression(parameter, propertyName);
            return BuildContainsExpression(propertyExpr, containsMethod, filterValue);
        }).ToList();
    }

    private static Expression BuildPropertyExpression(ParameterExpression parameter, string propertyPath)
    {
        Expression expression = parameter;
        foreach (var part in propertyPath.Split('.'))
        {
            expression = Expression.Property(expression, part);
        }
        return expression;
    }

    private static Expression BuildContainsExpression(
        Expression propertyExpr,
        MethodInfo containsMethod,
        string? filterValue)
    {
        if (filterValue == null)
            return Expression.Constant(false);

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        var stringExpr = propertyExpr.Type == typeof(string)
            ? propertyExpr
            : Expression.Convert(propertyExpr, typeof(string));

        return Expression.Call(
            Expression.Call(stringExpr, toLowerMethod),
            containsMethod,
            Expression.Constant(filterValue));
    }

    private static bool IsValidSortOrder(string? sortOrder)
    {
        return sortOrder != null &&
               (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}