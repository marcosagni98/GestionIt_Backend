using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IWorkLogRepository : IGenericRepository<WorkLog>
{
    /// <summary>
    /// Retrieves a list of work logs associated with a specific incident.
    /// </summary>
    /// <param name="incidentId">The unique identifier of the incident.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of work logs.</returns>
    public Task<List<WorkLog>> GetByIncidentIdAsync(long incidentId);
}
