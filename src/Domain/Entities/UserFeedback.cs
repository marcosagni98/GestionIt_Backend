using Domain.Entities.Common;

namespace Domain.Entities;

public class UserFeedback : Entity
{
    public long IncidentId { get; set; } 
    public long UserId { get; set; } 
    public string Feedback { get; set; } = string.Empty;
    public int Rating { get; set; } 
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Incident? Incident { get; set; }
    public User? User { get; set; }
}

