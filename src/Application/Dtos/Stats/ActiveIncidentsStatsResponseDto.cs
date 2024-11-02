

namespace Application.Dtos.Stats;

/// <summary>
/// DTO representing the count and severity of active incidents.
/// </summary>
/// <param name="Total">Total number of active incidents.</param>
/// <param name="High">Count of high-severity incidents.</param>
/// <param name="Medium">Count of medium-severity incidents.</param>
/// <param name="Low">Count of low-severity incidents.</param>
/// <param name="VariationFromLastMonth">Variation of incidences from last month</param>
public record ActiveIncidentsStatsResponseDto(
    int Total,
    int High,
    int Medium,
    int Low,
    double VariationFromLastMonth
);