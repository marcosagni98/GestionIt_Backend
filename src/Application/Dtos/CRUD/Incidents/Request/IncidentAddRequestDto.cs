using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// Incident add request dto
/// </summary>
public class IncidentAddRequestDto
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
    /// User id that created the incident
    /// </summary>
    [JsonIgnore]
    public long? UserId { get; set; }
}
