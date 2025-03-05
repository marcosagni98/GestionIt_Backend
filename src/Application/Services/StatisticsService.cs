using Application.Dtos.Stats;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Initializes a new instance of the <see cref="StatisticsService"/> class.
/// </summary>
/// <param name="logger"></param>
/// <param name="userRepository"></param>
/// <param name="incidentRepository"></param>
/// <param name="userFeedbackRepository"></param>
public class StatisticsService(ILogger<StatisticsService> logger, IUserRepository userRepository, IIncidentRepository incidentRepository, IUserFeedbackRepository userFeedbackRepository) : IStatisticsService
{
    private readonly ILogger<StatisticsService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;
    private readonly IUserFeedbackRepository _userFeedbackRepository = userFeedbackRepository;

    /// <inheritdoc/>
    public async Task<Result<ActiveIncidentsStatsResponseDto>> GetActiveIncidentsSevirityCount(long id)
    {
        User? user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            string error = $"User with id {id} not found";
            _logger.LogError(error);
            return Result.Fail(error);
        }

        int totalCount, lowCount, mediumCount, highCount;
        double VariationFromLastMonth;
        if (user.UserType == UserType.Admin )
        {
            lowCount = await _incidentRepository.CountByPriorityAsync(Priority.Low);
            mediumCount = await _incidentRepository.CountByPriorityAsync(Priority.Medium);
            highCount = await _incidentRepository.CountByPriorityAsync(Priority.High);
            totalCount = lowCount + mediumCount + highCount;
            VariationFromLastMonth = await CalculateRatioOfNewIncidentsFromLastMonth();
        }
        else if (user.UserType == UserType.Technician)
        {
            lowCount = await _incidentRepository.CountByPriorityAsync(Priority.Low, id);
            mediumCount = await _incidentRepository.CountByPriorityAsync(Priority.Medium, id);
            highCount = await _incidentRepository.CountByPriorityAsync(Priority.High, id);
            totalCount = lowCount + mediumCount + highCount;
            VariationFromLastMonth = await CalculateRatioOfNewIncidentsFromLastMonth(id);
        }
        else
        {
            string error = "User not authorized";
            _logger.LogError(error);
            return Result.Fail<ActiveIncidentsStatsResponseDto>(error);
        }

        return Result.Ok(new ActiveIncidentsStatsResponseDto(totalCount, highCount, mediumCount, lowCount, VariationFromLastMonth));
    }

    #region GetActiveIncidentsSevirityCount private methods

    /// <summary>
    /// Calculates the ratio of new incidents created in the current month compared to the previous month.
    /// </summary>
    /// <returns>A asyncronous task that represnts the variation of new incidents created current month to last month</returns>
    private async Task<double> CalculateRatioOfNewIncidentsFromLastMonth(long id)
    {
        var (startOfLast30Days, endOfLast30Days) = GetDateRange(30, 0);
        var (startOfPrevious30Days, endOfPrevious30Days) = GetDateRange(60, 31);

        int currentMonthIncidents = await _incidentRepository.CountAsync(startOfLast30Days, endOfLast30Days, id);
        int previousMonthIncidents = await _incidentRepository.CountAsync(startOfPrevious30Days, endOfPrevious30Days, id);

        double VariationFromLastMonth = CalculateChangeRatio(currentMonthIncidents, previousMonthIncidents);
        return VariationFromLastMonth;
    }

    /// <summary>
    /// Calculates the ratio of new incidents created in the current month compared to the previous month.
    /// </summary>
    /// <returns>A asyncronous task that represnts the variation of new incidents created current month to last month</returns>
    private async Task<double> CalculateRatioOfNewIncidentsFromLastMonth()
    {
        var (startOfLast30Days, endOfLast30Days) = GetDateRange(30, 0);
        var (startOfPrevious30Days, endOfPrevious30Days) = GetDateRange(60, 31);

        int currentMonthIncidents = await _incidentRepository.CountAsync(startOfLast30Days, endOfLast30Days);
        int previousMonthIncidents = await _incidentRepository.CountAsync(startOfPrevious30Days, endOfPrevious30Days);

        double VariationFromLastMonth = CalculateChangeRatio(currentMonthIncidents, previousMonthIncidents);
        return VariationFromLastMonth;
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<AverageIncidencesResolutionTimeResponseDto>> GetAverageResolutionTime(long id)
    {
        User? user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Fail<AverageIncidencesResolutionTimeResponseDto>("User not found");
        }
        var (startOfLast30Days, endOfLast30Days) = GetDateRange(30, 0);
        var (startOfPrevious30Days, endOfPrevious30Days) = GetDateRange(60, 31);

        double currentMonthResolutionTime, previousMonthResolutionTime;
        if (user.UserType == UserType.Admin)
        {
            currentMonthResolutionTime = await _incidentRepository.GetAverageResolutionTimeAsync(startOfLast30Days, endOfLast30Days);
            previousMonthResolutionTime = await _incidentRepository.GetAverageResolutionTimeAsync(startOfPrevious30Days, endOfPrevious30Days);
        }
        else if(user.UserType == UserType.Technician)
        {
            currentMonthResolutionTime = await _incidentRepository.GetAverageResolutionTimeAsync(startOfLast30Days, endOfLast30Days, id);
            previousMonthResolutionTime = await _incidentRepository.GetAverageResolutionTimeAsync(startOfPrevious30Days, endOfPrevious30Days, id);
        }
        else
        {
            return Result.Fail<AverageIncidencesResolutionTimeResponseDto>("User not authorized");
        }

        double changeRatio = CalculateChangeRatio(currentMonthResolutionTime, previousMonthResolutionTime);
        int currentMonthResolutionTimeRedondeado = (int)Math.Round(currentMonthResolutionTime);
        return Result.Ok(new AverageIncidencesResolutionTimeResponseDto(currentMonthResolutionTimeRedondeado, changeRatio));
    }

    /// <inheritdoc/>
    public async Task<Result<UserHappinessResponseDto>> GetUserHappinessAsync(long id)
    {
        User? user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Fail<UserHappinessResponseDto>("User not found");
        }

        var (startOfLast30Days, endOfLast30Days) = GetDateRange(30, 0);
        var (startOfPrevious30Days, endOfPrevious30Days) = GetDateRange(60, 31);

        double currentMonthRatio, previousMonthRatio;
        if (user.UserType == UserType.Admin)
        {
            currentMonthRatio = await GetUserHappinessForMonth(startOfLast30Days, endOfLast30Days);
            previousMonthRatio = await GetUserHappinessForMonth(startOfPrevious30Days, endOfPrevious30Days);
        }
        else if (user.UserType == UserType.Technician)
        {
            currentMonthRatio = await GetUserHappinessForMonth(startOfLast30Days, endOfLast30Days, id);
            previousMonthRatio = await GetUserHappinessForMonth(startOfPrevious30Days, endOfPrevious30Days, id);
        }
        else
        {
            return Result.Fail<UserHappinessResponseDto>("User not authorized");
        }

        double changeRatio = CalculateChangeRatio(currentMonthRatio, previousMonthRatio);

        return Result.Ok(new UserHappinessResponseDto(currentMonthRatio, changeRatio));
    }
    #region GetUserHappinessAsync private methods
    /// <summary>
    /// Retrieves the average happiness score within a specified date range, normalized to a ratio between 0 and 1.
    /// </summary>
    /// <param name="start">The start date of the period for which to calculate happiness.</param>
    /// <param name="end">The end date of the period for which to calculate happiness.</param>
    /// <returns>A double representing the average happiness ratio for the specified period, normalized between 0 and 1.</returns>
    private async Task<double> GetUserHappinessForMonth(DateTime start, DateTime end)
    {
        int happinessValue = await _userFeedbackRepository.GetUserHappinessAsync(start, end);
        return happinessValue / 5.0;
    }

    /// <summary>
    /// Retrieves the average happiness score within a specified date range, normalized to a ratio between 0 and 1.
    /// </summary>
    /// <param name="start">The start date of the period for which to calculate happiness.</param>
    /// <param name="end">The end date of the period for which to calculate happiness.</param>
    /// <param name="id">The ID of the user whose happiness is to be calculated.</param>
    /// <returns>A double representing the average happiness ratio for the specified period, normalized between 0 and 1.</returns>
    private async Task<double> GetUserHappinessForMonth(DateTime start, DateTime end, long id)
    {
        int happinessValue = await _userFeedbackRepository.GetUserHappinessAsync(start, end, id);
        return happinessValue / 5.0;
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<IncidencesResumeResponseDto>> GetIncidencesResumeAsync()
    {
        var unassigned = await _incidentRepository.CountByStatusAsync(Status.Unassigned);
        var closed = await _incidentRepository.CountByStatusAsync(Status.Completed);
        int open = await GetOpenIncidentsAsync();
        return Result.Ok(new IncidencesResumeResponseDto(open, closed, unassigned));
    }
    #region GetIncidencesResumeAsync private methods
    /// <summary>
    /// Gets the number of open incidents.
    /// </summary>
    /// <returns>A asyncronous task that represents the number of open incidents</returns>
    private async Task<int> GetOpenIncidentsAsync()
    {
        var pending = await _incidentRepository.CountByStatusAsync(Status.Pending);
        var inProgress = await _incidentRepository.CountByStatusAsync(Status.InProgress);
        var review = await _incidentRepository.CountByStatusAsync(Status.Review);

        var open = pending + inProgress + review;
        return open;
    }
    #endregion

    /// <inheritdoc/>
    public async Task<Result<IncidencesMonthlyResumeResponseDto>> GetIncidencesMonthlyResumeAsync()
    {
        var endDate = DateTime.Now;
        var startDate = endDate.AddMonths(-6);

        // Obtener incidencias por mes para los últimos 6 meses
        var incidencesByMonth = await GetIncidencesByMonthAsync(endDate, 6);

        int totalIncidences = incidencesByMonth.Values.Sum();

        // Calcular el ratio de cambio en los últimos 6 meses comparado con el período anterior
        decimal changeRatioLast6Months = await CalculateChangeRatioAsync(startDate, totalIncidences);

        var resultDto = new IncidencesMonthlyResumeResponseDto(
            IncidencesByMonth: incidencesByMonth,
            ChangeRatioLast6Months: changeRatioLast6Months,
            Count: totalIncidences
        );

        return Result.Ok(resultDto);
    }

    #region GetIncidencesMonthlyResumeAsync private methods

    /// <summary>
    /// Retrieves the number of incidents grouped by month over a specified number of months,
    /// starting from a given end date and moving backwards.
    /// </summary>
    /// <param name="endDate">The end date for the data retrieval, typically the current date.</param>
    /// <param name="months">The number of months to retrieve data for, counting backwards from the end date.</param>
    /// <returns>A dictionary where each key is a month (represented as an integer) and each value is the count of incidents for that month.</returns>
    private async Task<Dictionary<int, int>> GetIncidencesByMonthAsync(DateTime endDate, int months)
    {
        var incidencesByMonth = new Dictionary<int, int>();

        for (int i = 0; i < months; i++)
        {
            var monthStart = endDate.AddMonths(-i).AddDays(1 - endDate.Day).Date;
            var monthEnd = new DateTime(monthStart.Year, monthStart.Month, DateTime.DaysInMonth(monthStart.Year, monthStart.Month));
            int count = await _incidentRepository.CountAsync(monthStart, monthEnd);
            incidencesByMonth[monthStart.Month] = count;
        }

        return incidencesByMonth;
    }

    /// <summary>
    /// Calculates the change ratio of incidents between a current period and a previous period of equal length.
    /// </summary>
    /// <param name="startDate">The start date of the current period to calculate incidents for (usually 6 months before the current date).</param>
    /// <param name="totalIncidences">The total number of incidents counted in the current period.</param>
    /// <returns>
    /// The change ratio as a decimal, calculated as the difference in incident counts between the current period and the previous period,
    /// divided by the count of the previous period. Returns 0 if there are no incidents in the previous period.
    /// </returns>
    private async Task<decimal> CalculateChangeRatioAsync(DateTime startDate, int totalIncidences)
    {
        var previousStartDate = startDate.AddMonths(-6);
        var previousEndDate = startDate;
        int previousCount = await _incidentRepository.CountAsync(previousStartDate, previousEndDate);

        return previousCount > 0
            ? (decimal)(totalIncidences - previousCount) / previousCount
            : 0;
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<List<IncidencesDailyResumeResponseDto>>> GetIncidencesDayResumeAsync()
    {
        var currentYear = DateTime.Now.Year;
        var dailyIncidencesTuples = await _incidentRepository.GetIncidentCountByDayAsync(currentYear);

        var dailyIncidences = dailyIncidencesTuples
            .Select(tuple => new IncidencesDailyResumeResponseDto(
                Date: tuple.Date,
                Count: tuple.Count
            ))
            .ToList();

        return Result.Ok(dailyIncidences);
    }


    #region private methods
    /// <summary>
    /// Calculates the date range between a specified number of days in the past and today.
    /// </summary>
    /// <param name="daysAgoStart">The number of days ago to start the range.</param>
    /// <param name="daysAgoEnd">The number of days ago to end the range.</param>
    /// <returns>A tuple containing the start and end dates for the specified range.</returns>
    private (DateTime Start, DateTime End) GetDateRange(int daysAgoStart, int daysAgoEnd)
    {
        DateTime endDate = DateTime.UtcNow.AddDays(-daysAgoEnd);
        DateTime startDate = DateTime.UtcNow.AddDays(-daysAgoStart);
        return (startDate, endDate);
    }

    /// <summary>
    /// Calculates the percentage change ratio between two happiness ratios.
    /// </summary>
    /// <param name="currentRatio">The happiness ratio for the current period.</param>
    /// <param name="previousRatio">The happiness ratio for the previous period.</param>
    /// <returns>The percentage change from the previous ratio to the current ratio as a double. 
    /// Returns 0 if the previous ratio is 0 to avoid division by zero.</returns>
    private double CalculateChangeRatio(double currentRatio, double previousRatio)
    {
        return previousRatio == 0 ? 0 : (currentRatio - previousRatio) / previousRatio;
    }

    /// <summary>
    /// Calculates the percentage change ratio between two happiness ratios.
    /// </summary>
    /// <param name="currentRatio">The happiness ratio for the current period.</param>
    /// <param name="previousRatio">The happiness ratio for the previous period.</param>
    /// <returns>The percentage change from the previous ratio to the current ratio as a double. 
    /// Returns 0 if the previous ratio is 0 to avoid division by zero.</returns>
    private double CalculateChangeRatio(int currentRatio, int previousRatio)
    {
        int ratio = previousRatio == 0 ? 0 : (currentRatio - previousRatio) / previousRatio;
        return ratio / 100.0;
    }
    #endregion
}
