using Application.Dtos.Stats;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// Statistics Service Interface
/// </summary>
public interface IStatisticsService : IDisposable
{
    /// <summary>
    /// Gets the count and severity of active incidents.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{ActiveIncidentsStatsResponseDto}"/> containing the count and severity of active incidents.
    /// </returns>
    public Task<Result<ActiveIncidentsStatsResponseDto>> GetActiveIncidentsSevirityCount(long id);

    /// <summary>
    /// Gets the average resolution time for incidences and the change ratio from the last month.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{AverageIncidencesResolutionTimeResponseDto}"/> containing the average resolution time and change ratio.
    /// </returns>
    public Task<Result<AverageIncidencesResolutionTimeResponseDto>> GetAverageResolutionTime(long id);

    /// <summary>
    /// Gets the user happiness statistics.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{UserHappinessResponseDto}"/> containing the user happiness ratio and change ratio from the last month.
    /// </returns>
    public Task<Result<UserHappinessResponseDto>> GetUserHappinessAsync(long id);

    /// <summary>
    /// Gets the summary of incidences.
    /// </summary>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{IncidencesResumeRequestDto}"/> containing the summary of incidences.
    /// </returns>
    public Task<Result<IncidencesResumeRequestDto>> GetIncidencesResumeAsync();
}
