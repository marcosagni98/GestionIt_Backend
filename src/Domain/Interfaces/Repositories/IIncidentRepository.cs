﻿using Domain.Entities;
using Domain.Enums;

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

    /// <summary>
    /// Counts the number of active incidents (not closed or completed) with the specified priority.
    /// </summary>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of incidents with the specified priority.</returns>
    public Task<int> CountByPriority(Priority priority);

    /// <summary>
    /// Counts the number of incidents with the specified status.
    /// </summary>
    /// <param name="status"><see cref="Status"/> of the incidents</param>
    /// <returns>A task representing the asynchronous operation, containing the count of incidents with the specified status.</returns>
    public Task<int> CountByStatus(Status status);

    /// <summary>
    /// Asynchronously counts the number of entities in the database in a period of time.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>A task representing the asynchronous count operation, returning the total count of entities.</returns>
    public Task<int> CountAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Asynchronously retrieves the average resolution time of incidents within a specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>A task representing the asynchronous count operation, returning the time of resolution</returns>
    public Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate);
}
