using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
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
}
