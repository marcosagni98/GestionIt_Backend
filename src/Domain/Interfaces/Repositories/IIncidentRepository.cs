using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing incident data.
/// </summary>
public interface IIncidentRepository : IGenericRepository<Incident>
{
    /// <summary>
    /// Retrieves a list of incidents associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose incidents are to be retrieved.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of 
    /// <see cref="Incident"/> objects associated with the specified user ID, 
    /// or null if no incidents are found.
    /// </returns>
    public Task<List<Incident>?> GetByUserIdAsync(long userId);
}
