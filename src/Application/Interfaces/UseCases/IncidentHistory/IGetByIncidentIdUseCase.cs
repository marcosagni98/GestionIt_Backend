using Application.Dtos.CRUD.IncidentHistories;

namespace Application.Interfaces.UseCases.IncidentHistory;

/// <summary>
/// Interface for the GetByIncidentIdUseCase use case.
/// </summary>
public interface IGetByIncidentIdUseCase
{
    /// <summary>
    /// Gets a list of incident history by incident
    /// </summary>
    /// <param name="incidentId">The id of the incident </param>
    /// <returns>An asynchronous tasks representing a <see cref="Result{List{IncidentHistory}}"/> of incidents histories assigned to an incident</returns>
    public Task<Result<List<IncidentHistoryDto>>> GetByIncidentIdAsync(long incidentId);
}