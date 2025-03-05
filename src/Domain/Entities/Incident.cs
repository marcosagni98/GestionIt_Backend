using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Incident : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Low;
    public Status Status { get; set; } = Status.Unassigned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long UserId { get; set; }
    public long? TechnicianId { get; set; } = null;

    public User User { get; set; } = null!;
    public User Technician { get; set; } = null!;
    public ICollection<IncidentHistory> IncidentHistories { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<WorkLog> WorkLogs { get; set; } = [];
    public ICollection<UserFeedback> UserFeedbacks { get; set; } = [];
}

