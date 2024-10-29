namespace Application.Dtos.CRUD.WorkLogs.Request;

/// <summary>
/// Work log update request dto
/// </summary>
public class WorkLogUpdateRequestDto
{
    /// <summary>
    /// The id of the incident
    /// </summary>
    public long IncidentId { get; set; }

    /// <summary>
    /// Id of the technician
    /// </summary>
    public long TechnicianId { get; set; }

    /// <summary>
    /// The time worked on the incident
    /// </summary>
    public decimal MinWorked { get; set; }

    /// <summary>
    /// Date of the log
    /// </summary>
    public DateTime LogDate { get; set; }
}
