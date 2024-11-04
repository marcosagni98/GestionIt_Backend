using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Stats;

/// <summary>
/// DTO for summarizing incident statistics on a monthly basis.
/// </summary>
/// <param name="IncidencesByMonth">A list of monthly incident counts. Each entry contains the month and the corresponding incident count.</param>
/// <param name="ChangeRatioLast6Months">The change ratio of incidents over the last 6 months, expressed as a decimal.</param>
/// <param name="Count">The total number of incidents recorded.</param>
public record IncidencesMonthlyResumeResponseDto(
    Dictionary<int,int> IncidencesByMonth,
    decimal ChangeRatioLast6Months,
    int Count
);
