namespace Application.Dtos.CRUD.UserFeedbacks;

/// <summary>
/// User feedback on an incident
/// </summary>
public class UserFeedbackDto
{
    /// <summary>
    /// Id of the feedback
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The id of the incident
    /// </summary>
    public long IncidentId { get; set; }

    /// <summary>
    /// The id of the user that submitted the feedback
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Name of the user that submitted the feeback
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The feedback text
    /// </summary>
    public string Feedback { get; set; } = string.Empty;

    /// <summary>
    /// Rating of the resolution of the incident
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Date and time the feedback was submitted
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
