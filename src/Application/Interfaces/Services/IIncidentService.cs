using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Enums;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// incident Service Interface
/// </summary>
public interface IIncidentService : IBaseService<IncidentDto, IncidentAddRequestDto, IncidentUpdateRequestDto>, IDisposable
{
    /// <summary>
    /// Gets all incident IDs associated with a specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose incident IDs are to be retrieved.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{List{long}}"/> containing the IDs of incidents 
    /// associated with the specified user ID.
    /// </returns>
    public Task<Result<List<long>>> GetIncidentIdsByUserIdAsync(long userId);

    /// <summary>
    /// Updates the status of an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="statusRequest">The status update request containing the new status and additional details.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{SuccessResponseDto}"/> indicating the success or failure of the operation.
    /// </returns>
    public Task<Result<SuccessResponseDto>> UpdateStatusAsync(long id, IncidentUpdateStatusRequestDto statusRequest);

    /// <summary>
    /// Gets all incidents assigned to a specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose assigned incidents are to be retrieved.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{List{IncidentDto}}"/> containing the incidents 
    /// assigned to the specified user ID.
    /// </returns>
    public Task<Result<PaginatedList<IncidentDto>>> GetAsync(QueryFilterDto queryFilter, long userId);

    /// <summary>
    /// Updates the technician assigned to an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="technitianRequestDto">The data of the technician to assign to the incident.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{SuccessResponseDto}"/> indicating the success or failure of the operation.
    /// </returns>
    public Task<Result<SuccessResponseDto>> UpdateTechnitianAsync(long id, IncidentUpdateTechnitianRequestDto technitianRequestDto);

    /// <summary>
    /// Updates the title and description of an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateTitleDescriptionRequestDto">The data transfer object containing the new title and description.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{SuccessResponseDto}"/> indicating the success or failure of the operation.
    /// </returns>
    public Task<Result<SuccessResponseDto>> UpdateTitleAndDescription(long id, IncidentUpdateTitleDescriptionRequestDto updateTitleDescriptionRequestDto);
}
