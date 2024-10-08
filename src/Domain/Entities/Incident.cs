using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Incident : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Low;
    public Status Status { get; set; } = Status.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long UserId { get; set; }
    public long? TechnicianId { get; set; }

    public User User { get; set; } = null!;
    public User Technician { get; set; } = null!;
    public ICollection<IncidentHistory> IncidentHistories { get; set; } = new List<IncidentHistory>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    public ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>(); 
}

