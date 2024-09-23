namespace Domain.Entities;

public class Message
{
    public long Id { get; set; }
    public long IncidentId { get; set; }
    public Incident? Incident { get; set; }
    public long SenderId { get; set; }
    public User? Sender { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;
}
