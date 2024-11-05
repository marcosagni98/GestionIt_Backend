using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserFeedbackRepository : IGenericRepository<UserFeedback>
{
    /// <summary>
    /// Gets the user happiness index within a date range.
    /// </summary>
    /// <param name="startDate">Start date of the range.</param>
    /// <param name="endDate">End date of the range.</param>
    /// <returns>User happiness index.</returns>
    public Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets the happiness index of a specific user within a date range.
    /// </summary>
    /// <param name="startDate">Start date of the range.</param>
    /// <param name="endDate">End date of the range.</param>
    /// <param name="id">User identifier.</param>
    /// <returns>User happiness index.</returns>
    public Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate, long id);

    /// <summary>
    /// Gets the user feedback by incident identifier.
    /// </summary>
    /// <param name="incidentId">Incident identifier.</param>
    /// <returns>User feedback.</returns>
    public Task<UserFeedback?> GetByIncidentIdAsync(long incidentId);
}
