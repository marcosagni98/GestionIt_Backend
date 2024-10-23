namespace Application.Dtos.CRUD.Messages;

/// <summary>
/// message
/// </summary>
public class MessageDto
{
    /// <summary>
    /// The id of the incident
    /// </summary>
    public long IncidentId { get; set; }

    /// <summary>
    /// Id of the user that sent the message
    /// </summary>
    public long SenderId { get; set; }

    /// <summary>
    /// Text of the message
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Date and time the message was sent
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
