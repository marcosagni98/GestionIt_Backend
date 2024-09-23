namespace Domain.Entities;

public class WorkLog
{
    public long Id { get; set; }
    public long IncidentId { get; set; } 
    public Incident? Incident { get; set; } 
    public long TechnicianId { get; set; } 
    public User? Technician { get; set; } 
    public decimal MinWorked { get; set; } 
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;
}
