﻿using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing incident data.
/// </summary>
public interface IIncidentRepository : IGenericRepository<Incident>
{
    /// <summary>
    /// Retrieves a list of all incidents.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing a list of all incidents or null if no incidents are found.</returns>
    public Task<List<long>?> GetIdsAsync();

    /// <summary>
    /// Retrieves a list of incidents associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose incidents are to be retrieved.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of 
    /// <see cref="Incident"/> objects associated with the specified user ID, 
    /// or null if no incidents are found.
    /// </returns>
    public Task<List<long>?> GetIdsByUserIdAsync(long userId);

    /// <summary>
    /// Counts the number of active incidents (not closed or completed) with the specified priority.
    /// </summary>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of incidents with the specified priority.</returns>
    public Task<int> CountByPriorityAsync(Priority priority);

    /// <summary>
    /// Counts the number of active incidents (not closed or completed) with the specified priority asigned to a user.
    /// </summary>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of incidents with the specified priority.</returns>
    public Task<int> CountByPriorityAsync(Priority priority, long technicianId);

    /// <summary>
    /// Counts the number of incidents with the specified status.
    /// </summary>
    /// <param name="status"><see cref="Status"/> of the incidents</param>
    /// <returns>A task representing the asynchronous operation, containing the count of incidents with the specified status.</returns>
    public Task<int> CountByStatusAsync(Status status);

    /// <summary>
    /// Asynchronously counts the number of entities in the database in a period of time.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>A task representing the asynchronous count operation, returning the total count of entities.</returns>
    public Task<int> CountAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Asynchronously counts the number of entities in the database in a period of time assigned to a user.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="id">The ID of the user whose incidents are to be retrieved.</param>
    /// <returns>A task representing the asynchronous count operation, returning the total count of entities.</returns>
    public Task<int> CountAsync(DateTime startDate, DateTime endDate, long id);

    /// <summary>
    /// Retrieves the historical (closed and completed) list of incidents.
    /// </summary>
    /// <param name="queryFilter">The query filter containing pagination, sorting, and search criteria.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of historical incidents.</returns>
    public Task<PaginatedList<Incident>> GetHistoricAsync(QueryFilterDto queryFilter);

    /// <summary>
    /// Retrieves a paginated list of incidents filtered by priority.
    /// </summary>
    /// <param name="queryFilter">The query filter containing pagination, sorting, and search criteria.</param>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated list of incidents filtered by the specified priority.</returns>
    public Task<PaginatedList<Incident>> GetByPriorityAsync(QueryFilterDto queryFilter, Priority priority);

    /// <summary>
    /// Retrieves a paginated list of incidents filtered by priority and user.
    /// </summary>
    /// <param name="queryFilter">The query filter containing pagination, sorting, and search criteria.</param>
    /// <param name="userId">The ID of the user whose incidents are to be retrieved.</param>
    /// <param name="priority">The priority level to filter incidents by.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated list of incidents filtered by the specified priority and user.</returns>
    public Task<PaginatedList<Incident>> GetIncidentsByPriorityUserAsync(QueryFilterDto queryFilter, Priority priority, long userId);

    /// <summary>
    /// Asynchronously retrieves the average resolution time of incidents within a specified date range.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>A task representing the asynchronous count operation, returning the time of resolution</returns>
    public Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Asynchronously retrieves the average resolution time of incidents within a specified date range, asigned to a user.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>A task representing the asynchronous count operation, returning the time of resolution</returns>
    public Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate, long id);

    /// <summary>
    /// Asynchronously updates the status of an incident by its ID.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="newStatus">The new status to set for the incident.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public Task UpdateIncidentStatusAsync(long id, Status newStatus);

    /// <summary>
    /// Asynchronously updates the priority of an incident by its ID.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="priority">The new priority to set for the incident.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public Task UpdateIncidentPriorityAsync(long id, Priority priority);

    /// <summary>
    /// Asynchronously retrieves a paginated list of incidents associated with a specific user, based on the provided query filter.
    /// </summary>
    /// <param name="queryFilter">The query filter containing pagination, sorting, and search criteria.</param>
    /// <param name="userId">The ID of the user whose incidents are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated list of incidents associated with the specified user.</returns>
    public Task<PaginatedList<Incident>> GetIncidentsOfUserAsync(QueryFilterDto queryFilter, long userId);

    /// <summary>
    /// Retrieves the daily summary of incidents for a specific year, including the date and the count of incidents for each day that had incidents.
    /// </summary>
    /// <param name="year">The year for which the incident counts are retrieved. Only incidents created in this year will be considered.</param>
    /// <returns>
    /// An asynchronous task that returns a list of tuples, each containing:
    /// - `Date`: A string representing the date in "yyyy-MM-dd" format.
    /// - `Count`: The count of incidents that occurred on that date. Only days with one or more incidents will be included in the result.
    /// </returns>
    public Task<List<(string Date, int Count)>> GetIncidentCountByDayAsync(int year);
}