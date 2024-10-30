using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// Base service interface that implements the basic functions needed for the API.
/// </summary>
/// <typeparam name="TDto">The type of the entity data transfer object.</typeparam>
/// <typeparam name="TResponseDto">The type of the response data transfer object.</typeparam>
/// <typeparam name="TAddRequestDto">The type of the data transfer object used for adding new entities.</typeparam>
/// <typeparam name="TUpdateRequestDto">The type of the data transfer object used for updating existing entities.</typeparam>
public interface IBaseService<TDto, TAddRequestDto, TUpdateRequestDto>
    where TDto : class
    where TAddRequestDto : class
    where TUpdateRequestDto : class
{
    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="addRequestDto">The data for the new entity.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{CreatedResponseDto}"/> indicating success or failure.</returns>
    Task<Result<CreatedResponseDto>> AddAsync(TAddRequestDto addRequestDto);

    /// <summary>
    /// Retrieves a list of entities asynchronously.
    /// </summary>
    /// <param name="queryFilter">The filtering, sorting, and pagination parameters.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="Result{PaginatedList{TDto}}"/> containing the list of entities.</returns>
    Task<Result<PaginatedList<TDto>>> GetAsync(QueryFilterDto queryFilter);

    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{TResponseDto}"/> containing the entity if found, or an error.</returns>
    Task<Result<TDto>> GetByIdAsync(long id);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="updateRequestDto">The updated data for the entity.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{SuccessResponseDto}"/> indicating success or failure.</returns>
    Task<Result<SuccessResponseDto>> UpdateAsync(long id, TUpdateRequestDto updateRequestDto);

    /// <summary>
    /// Deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to be deleted.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{SuccessResponseDto}"/> indicating success or failure.</returns>
    Task<Result<SuccessResponseDto>> DeleteAsync(long id);
}

