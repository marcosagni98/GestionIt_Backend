namespace Domain.Entities;

public class Incident
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long UserId { get; set; }
    public User? User { get; set; }
    public long? TechnicianId { get; set; } 
    public User? Technician { get; set; }
    public bool Active { get; set; } = true;

    // Relación con otras entidades
    public ICollection<IncidentHistory> IncidentHistories { get; set; } = new List<IncidentHistory>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    public ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();
}

