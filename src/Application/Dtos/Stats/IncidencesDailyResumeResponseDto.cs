namespace Application.Dtos.Stats;

/// <summary>  
/// Represents a daily summary of incidences with a date and count.  
/// </summary>  
/// <param name="Date">The date ("yyyy-MM-dd") of the incidences.</param>  
/// <param name="Count">The count of incidences on the specified date.</param>  
public record IncidencesDailyResumeResponseDto(
   string Date,
   int Count
);
