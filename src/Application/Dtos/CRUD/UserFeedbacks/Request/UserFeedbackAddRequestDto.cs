namespace Application.Dtos.CRUD.UserFeedbacks.Request;

/// <summary>
/// User feedback add request dto
/// </summary>
public class UserFeedbackAddRequestDto
{
    /// <summary>
    /// The id of the incident
    /// </summary>
    public long IncidentId { get; set; }

    /// <summary>
    /// The id of the user that submitted the feedback
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// The feedback text
    /// </summary>
    public string Feedback { get; set; } = string.Empty;

    /// <summary>
    /// Rating of the resolution of the incident
    /// </summary>
    public int Rating { get; set; }
}
