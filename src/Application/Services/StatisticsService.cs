using Application.Dtos.Stats;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class StatisticsService : IStatisticsService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    /// <param name="mapper">The mapper for object mapping.</param>
    public StatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    #region Dispose
    private bool disposed = false;

    /// <summary>
    /// Releases the unmanaged resources used by the service and optionally releases
    /// the managed resources if disposing is true.
    /// </summary>
    /// <param name="disposing">Indicates whether the method was called directly
    /// or from a finalizer. If true, the method has been called directly
    /// and managed resources should be disposed. If false, it was called by the
    /// runtime from inside the finalizer and only unmanaged resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }
        disposed = true;
    }

    /// <summary>
    /// Releases all resources used by the service.
    /// This method is called by consumers of the service when they are done
    /// using it to free resources promptly.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<ActiveIncidentsStatsResponseDto>> GetActiveIncidentsSevirityCount(long id)
    {
        User? user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if(user == null)
        {
            return Result.Fail<ActiveIncidentsStatsResponseDto>("User not found");
        }

        int totalCount, lowCount, mediumCount, highCount;
        double VariationFromLastMonth;
        if (user.UserType == UserType.Admin )
        {
            totalCount = await _unitOfWork.IncidentRepository.CountAsync(null, null, null);
            lowCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.Low);
            mediumCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.Medium);
            highCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.High);
            VariationFromLastMonth = await CalculateRatioOfNewIncidentsFromLastMonth();
        }
        else if (user.UserType == UserType.Technician)
        {
            totalCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.Low, id);
            lowCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.Low, id);
            mediumCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.Medium, id);
            highCount = await _unitOfWork.IncidentRepository.CountByPriorityAsync(Priority.High, id);
            VariationFromLastMonth = await CalculateRatioOfNewIncidentsFromLastMonth(id);
        }
        else
        {
            return Result.Fail<ActiveIncidentsStatsResponseDto>("User not authorized");
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

        int currentMonthIncidents = await _unitOfWork.IncidentRepository.CountAsync(startOfLast30Days, endOfLast30Days, id);
        int previousMonthIncidents = await _unitOfWork.IncidentRepository.CountAsync(startOfPrevious30Days, endOfPrevious30Days, id);

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

        int currentMonthIncidents = await _unitOfWork.IncidentRepository.CountAsync(startOfLast30Days, endOfLast30Days);
        int previousMonthIncidents = await _unitOfWork.IncidentRepository.CountAsync(startOfPrevious30Days, endOfPrevious30Days);

        double VariationFromLastMonth = CalculateChangeRatio(currentMonthIncidents, previousMonthIncidents);
        return VariationFromLastMonth;
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<AverageIncidencesResolutionTimeResponseDto>> GetAverageResolutionTime(long id)
    {
        User? user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Fail<AverageIncidencesResolutionTimeResponseDto>("User not found");
        }
        var (startOfLast30Days, endOfLast30Days) = GetDateRange(30, 0);
        var (startOfPrevious30Days, endOfPrevious30Days) = GetDateRange(60, 31);

        double currentMonthResolutionTime, previousMonthResolutionTime;
        if (user.UserType == UserType.Admin)
        {
            currentMonthResolutionTime = await _unitOfWork.IncidentRepository.GetAverageResolutionTimeAsync(startOfLast30Days, endOfLast30Days);
            previousMonthResolutionTime = await _unitOfWork.IncidentRepository.GetAverageResolutionTimeAsync(startOfPrevious30Days, endOfPrevious30Days);
        }
        else if(user.UserType == UserType.Technician)
        {
            currentMonthResolutionTime = await _unitOfWork.IncidentRepository.GetAverageResolutionTimeAsync(startOfLast30Days, endOfLast30Days, id);
            previousMonthResolutionTime = await _unitOfWork.IncidentRepository.GetAverageResolutionTimeAsync(startOfPrevious30Days, endOfPrevious30Days, id);
        }
        else
        {
            return Result.Fail<AverageIncidencesResolutionTimeResponseDto>("User not authorized");
        }

        double changeRatio = CalculateChangeRatio(currentMonthResolutionTime, previousMonthResolutionTime);

        return Result.Ok(new AverageIncidencesResolutionTimeResponseDto(currentMonthResolutionTime, changeRatio));
    }

    /// <inheritdoc/>
    public async Task<Result<UserHappinessResponseDto>> GetUserHappinessAsync(long id)
    {
        User? user = await _unitOfWork.UserRepository.GetByIdAsync(id);
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
        int happinessValue = await _unitOfWork.UserFeedbackRepository.GetUserHappinessAsync(start, end);
        return happinessValue / 100.0;
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
        int happinessValue = await _unitOfWork.UserFeedbackRepository.GetUserHappinessAsync(start, end, id);
        return happinessValue / 100.0;
    }

    #endregion

    /// <inheritdoc/>
    public async Task<Result<IncidencesResumeRequestDto>> GetIncidencesResumeAsync()
    {
        var unassigned = await _unitOfWork.IncidentRepository.CountByStatusAsync(Status.Unassigned);
        var closed = await _unitOfWork.IncidentRepository.CountByStatusAsync(Status.Completed);
        int open = await GetOpenIncidentsAsync();
        return Result.Ok(new IncidencesResumeRequestDto(open, closed, unassigned));
    }
    #region GetIncidencesResumeAsync private methods
    /// <summary>
    /// Gets the number of open incidents.
    /// </summary>
    /// <returns>A asyncronous task that represents the number of open incidents</returns>
    private async Task<int> GetOpenIncidentsAsync()
    {
        var pending = await _unitOfWork.IncidentRepository.CountByStatusAsync(Status.Pending);
        var inProgress = await _unitOfWork.IncidentRepository.CountByStatusAsync(Status.InProgress);
        var review = await _unitOfWork.IncidentRepository.CountByStatusAsync(Status.Review);

        var open = pending + inProgress + review;
        return open;
    }
    #endregion

    /// <inheritdoc/>
    public async Task<Result<IncidencesMonthlyResumeRequestDto>> GetIncidencesMonthlyResumeAsync()
    {
        // Define the start date as six months ago and the current date as the end date
        var endDate = DateTime.Now;
        var startDate = endDate.AddMonths(-6);

        // Fetch incidents count by month
        var incidencesByMonth = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++)
        {
            var monthStart = endDate.AddMonths(-i).Date;
            var monthEnd = new DateTime(monthStart.Year, monthStart.Month, DateTime.DaysInMonth(monthStart.Year, monthStart.Month));
            var count = await _unitOfWork.IncidentRepository.CountAsync(monthStart, monthEnd);
            incidencesByMonth[monthStart.Month] = count;
        }

        // Calculate the total count of incidents in the last 6 months
        int totalIncidences = incidencesByMonth.Values.Sum();

        // Calculate the change ratio by comparing counts from the previous 6 months with the current 6 months
        var previousStartDate = startDate.AddMonths(-6);
        var previousEndDate = startDate;
        var previousCount = await _unitOfWork.IncidentRepository.CountAsync(previousStartDate, previousEndDate);

        // Avoid division by zero in case previous count is zero
        decimal changeRatioLast6Months = previousCount > 0
            ? (decimal)(totalIncidences - previousCount) / previousCount
            : 0;

        // Create and return the DTO
        var resultDto = new IncidencesMonthlyResumeRequestDto(
            IncidencesByMonth: incidencesByMonth,
            ChangeRatioLast6Months: changeRatioLast6Months,
            Count: totalIncidences
        );

        return Result.Ok(resultDto);
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
