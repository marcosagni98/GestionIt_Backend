using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    /// <summary>
    /// Retrieves a list of messages associated with a specific incident.
    /// </summary>
    /// <param name="incidentId">The unique identifier of the incident.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of messages.</returns>
    public Task<List<Message>> GetByIncidentIdAsync(long incidentId);
}
