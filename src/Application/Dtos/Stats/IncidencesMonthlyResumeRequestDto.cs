using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Stats;

/// <summary>
/// DTO for summarizing incidences statistics.
/// </summary>
/// <param name="IncidencesByMonth">Dictionary containing the number of incidences per month.</param>
/// <param name="ChangeRatioLast6Months">Decimal representing the change ratio of incidences in the last 6 months.</param>
/// <param name="Count">Total count of incidences.</param>
public record IncidencesMonthlyResumeRequestDto(
    Dictionary<int, int> IncidencesByMonth,
    decimal ChangeRatioLast6Months,
    int Count
);
