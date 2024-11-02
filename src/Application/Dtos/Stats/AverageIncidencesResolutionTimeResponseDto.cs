namespace Application.Dtos.Stats;

/// <summary>
/// Represents the average resolution time for incidences and the change ratio from the last month.
/// </summary>
/// <param name="AvgTimeMin">The average time in minutes to resolve incidences.</param>
/// <param name="ChangeRatioFromLastMonth">The percentage change in average resolution time compared to the previous month.</param>
public record AverageIncidencesResolutionTimeResponseDto(
    double AvgTimeMin,
    double ChangeRatioFromLastMonth
);
