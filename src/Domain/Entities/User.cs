namespace Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
    public string UserType { get; set; } = string.Empty;    
    public bool Active { get; set; }

    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    public ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();
}

