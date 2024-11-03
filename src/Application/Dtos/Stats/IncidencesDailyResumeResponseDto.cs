namespace Application.Dtos.Stats;

/// <summary>  
/// Represents a daily summary of incidences with a date and count.  
/// </summary>  
/// <param name="Date">The date of the incidences.</param>  
/// <param name="Count">The count of incidences on the specified date.</param>  
public record IncidencesDailyResumeResponseDto(
   DateTime Date,
   int Count
);
