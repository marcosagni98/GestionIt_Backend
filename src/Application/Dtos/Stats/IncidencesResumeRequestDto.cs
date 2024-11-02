namespace Application.Dtos.Stats;

/// <summary>
/// DTO for summarizing incidences statistics.
/// </summary>
/// <param name="OpenedIncidences">Number of incidences that are currently open.</param>
/// <param name="ClosedIncidences">Number of incidences that have been closed (only the completed).</param>
/// <param name="UnassignedIncidences">Number of incidences that are unassigned.</param>
public record IncidencesResumeRequestDto(
    int OpenedIncidences,
    int ClosedIncidences,
    int UnassignedIncidences
);
