using Application.Dtos.CommonDtos;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// Interface for message service operations
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="addRequestDto">The data for the new entity.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{CreatedResponseDto}"/> indicating success or failure.</returns>
    public Task<Result<CreatedResponseDto>> AddAsync(MessageAddRequestDto addRequestDto);

    /// <summary>
    /// Retrieves a list of messages associated with a specific incident ID.
    /// </summary>
    /// <param name="incidentId">The ID of the incident.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of MessageDto objects.</returns>
    public Task<Result<List<MessageDto>>> GetByIncidentIdAsync(long incidentId);
}
