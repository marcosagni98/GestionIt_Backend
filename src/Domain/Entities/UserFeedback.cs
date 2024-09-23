namespace Domain.Entities;

public class UserFeedback
{
    public long Id { get; set; }
    public long IncidentId { get; set; } 
    public Incident? Incident { get; set; } 
    public long UserId { get; set; } 
    public User? User { get; set; } 
    public string Feedback { get; set; } = string.Empty;
    public int Rating { get; set; } 
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;
}

