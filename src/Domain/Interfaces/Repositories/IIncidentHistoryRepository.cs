using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Entities.Common;

namespace Domain.Interfaces.Repositories;

public interface IIncidentHistoryRepository
{
    /// <summary>
    /// Asynchronously adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    public Task AddAsync(IncidentHistory entity);

    /// <summary>
    /// Asynchronously retrieves a paginated list of entities from the database, applying optional query filters for sorting and searching.
    /// </summary>
    /// <param name="queryFilter">Query filter data for sorting, searching, and pagination.</param>
    /// <returns>A task representing the asynchronous operation, returning a paginated list of entities.</returns>
    public Task<PaginatedList<IncidentHistory>> GetAsync(QueryFilterDto queryFilter);

    /// <summary>
    /// Asynchronously retrieves a specific entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning the entity if found, or null if not.</returns>
    public Task<IncidentHistory?> GetByIdAsync(long id);

    /// <summary>
    /// Asynchronously updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The updated data of the entity.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public Task UpdateAsync(IncidentHistory entity);

    /// <summary>
    /// Asynchronously counts the number of entities in the database based on optional filtering criteria.
    /// </summary>
    /// <param name="queryFilter">Query filter data for sorting, searching, and pagination.</param>
    /// <param name="filterParameter">Additional parameters to filter the count operation.</param>
    /// <returns>A task representing the asynchronous count operation, returning the total count of entities.</returns>
    public Task<int> CountAsync(QueryFilterDto queryFilter, List<string>? filterParameter);

    /// <summary>
    /// Asynchronously checks whether an entity with the specified identifier exists in the database.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the entity exists, otherwise false.</returns>
    public Task<bool> ExistsAsync(long id);
}
