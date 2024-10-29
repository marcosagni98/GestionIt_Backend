using Domain.Enums;

namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// incident update request dto
/// </summary>
public class IncidentUpdateRequestDto
{
    /// <summary>
    /// Title of the incident
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of the incident
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Priority of the incident
    /// </summary>
    public Priority? Priority { get; set; }

    /// <summary>
    /// Status of the incident
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// When the incident was created
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// User id that created the incident
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Id of the technician assigned to the incident
    /// </summary>
    public long? TechnicianId { get; set; }
}
