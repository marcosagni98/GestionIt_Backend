using Domain.Entities.Common;

namespace Domain.Entities;

public class Message : Entity
{
    public long IncidentId { get; set; }
    public long SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public User? Sender { get; set; }
    public Incident? Incident { get; set; }
}
