using Application.Dtos.CRUD.IncidentHistories;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// IncidentHistory Service Interface
/// </summary>
public interface IIncidentHistoryService : IDisposable
{
    /// <summary>
    /// Gets a list of incdent history by incident
    /// </summary>
    /// <param name="incidentId">The id of the incident </param>
    /// <returns>A asyncronous tasks reprenseting a <see cref="Result{List{IncidentHistory}}"/> of incidents histories asigned to a incident</returns>
    public Task<Result<List<IncidentHistoryDto>>> GetByIncidentIdAsync(long incidentId);

}
