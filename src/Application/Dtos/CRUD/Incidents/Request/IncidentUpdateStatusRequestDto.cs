using Domain.Enums;

namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// Represents an incident status update request.
/// </summary>
/// <param name="StatusId">The id of the status</param>
/// <param name="ChangedBy">The id of the user that changes the status</param>
/// <param name="ResolutionDetails">Text of the resolution</param>
public record IncidentUpdateStatusRequestDto(
    Status StatusId,
    long ChangedBy,
    string? ResolutionDetails);
