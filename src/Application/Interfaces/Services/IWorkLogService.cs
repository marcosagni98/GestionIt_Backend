using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Request;
using FluentResults;


namespace Application.Interfaces.Services;

/// <summary>
/// Retrieves a list of work logs associated with a specific incident.
/// </summary>
public interface IWorkLogService : IBaseService<WorkLogDto, WorkLogAddRequestDto, WorkLogUpdateRequestDto>, IDisposable
{
    /// <summary>
    /// Retrieves a list of work logs associated with a specific incident.
    /// </summary>
    /// <param name="incidentId">The ID of the incident.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of work log DTOs.</returns>
    public Task<Result<List<WorkLogDto>>> GetByIncidentIdAsync(long incidentId);
}
