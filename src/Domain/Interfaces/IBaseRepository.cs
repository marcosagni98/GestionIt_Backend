using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;

namespace Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Adds an entity to the database.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>A task representing the asynchronous add operation, with a <see cref="Result{TEntity}"/> indicating success or failure.</returns>
    public Task<Result<Unit>> AddAsync(TEntity entity);

    /// <summary>
    /// Retrieves all entities from the database based on the provided query filter.
    /// </summary>
    /// <param name="queryFilter">Query filter data for sorting, searching, and pagination.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="Result{PaginatedList{TEntity}}"/> with the entities.</returns>
    public Task<Result<PaginatedList<TEntity>>> GetAsync(QueryFilterDto queryFilter);

    /// <summary>
    /// Retrieves the entity with the specified id.
    /// </summary>
    /// <param name="id">The id of the entity to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="Result{TEntity}"/> if found.</returns>
    public Task<Result<TEntity>> GetByIdAsync(int id);

    /// <summary>
    /// Updates the entity in the database.
    /// </summary>
    /// <param name="entity">The data of the entity to be updated.</param>
    /// <returns>A task representing the asynchronous update operation, with a <see cref="Result{TEntity}"/> indicating success or failure.</returns>
    public Task<Result<Unit>> UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes the entity with the specified id.
    /// </summary>
    /// <param name="id">The id of the entity to be deleted.</param>
    /// <returns>A task representing the asynchronous delete operation, with a <see cref="Result{TEntity}"/> indicating success or failure.</returns>
    public Task<Result<Unit>> DeleteAsync(long id);

    /// <summary>
    /// Counts the number of entities in the database based on the provided filter.
    /// </summary>
    /// <param name="queryFilter">Query filter data for sorting, searching, and pagination.</param>
    /// <param name="filterParameter">Parameters to filter the results.</param>
    /// <returns>A task representing the asynchronous count operation, containing a <see cref="Result{int}"/> with the count.</returns>
    public Task<Result<int>> CountAsync(QueryFilterDto queryFilter, List<string>? filterParameter);

    /// <summary>
    /// Checks if an entity with the specified id exists in the database.
    /// </summary>
    /// <param name="id">The id of the entity to check.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="Result{bool}"/> indicating whether the entity exists.</returns>
    public Task<Result<bool>> ExistsAsync(int id);
}
