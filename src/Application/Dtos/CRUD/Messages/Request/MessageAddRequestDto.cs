namespace Application.Dtos.CRUD.Messages.Request;

/// <summary>
/// Represents a chat message within a specific incident group.
/// </summary>
/// <param name="IncidentId">The ID of the incident this message belongs to.</param>
/// <param name="SenderId">The ID of the user who sent the message.</param>
/// <param name="Text">The content of the message.</param>
/// <param name="SentAt">The date and time when the message was sent.</param>
public record MessageAddRequestDto(
    long IncidentId,
    long SenderId,
    string Text,
    DateTime SentAt
);
