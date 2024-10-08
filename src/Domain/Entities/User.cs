using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
    public UserType UserType { get; set; } = UserType.Basic;    

    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    public ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();
}

