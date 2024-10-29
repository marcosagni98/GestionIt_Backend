using Domain.Enums;

namespace Application.Dtos.CRUD.IncidentHistories.Request;

/// <summary>
/// Incident history add dto
/// </summary>
public class IncidentHistoryAddRequestDto
{
    /// <summary>
    /// Id of the incident
    /// </summary>
    public long? IncidentId { get; set; }

    /// <summary>
    /// Status of the incident
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// Date and time the incident was changed
    /// </summary>
    public DateTime? ChangedAt { get; set; }

    /// <summary>
    /// User id that changed the incident
    /// </summary>
    public long? ChangedBy { get; set; }

    /// <summary>
    /// Details of the resolution
    /// </summary>
    public string? ResolutionDetails { get; set; }
}
