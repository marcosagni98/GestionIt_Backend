namespace Application.Dtos.Stats;

/// <summary>
/// Represents the response data transfer object for user happiness statistics.
/// </summary>
/// <param name="HappinessRatio">The happiness ratio of the user.</param>
/// <param name="ChangeRatioFromLastMonth">The change in happiness ratio from the last month.</param>
public record UserHappinessResponseDto(
    double HappinessRatio,
    double ChangeRatioFromLastMonth
);
