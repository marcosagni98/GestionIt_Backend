using Domain.Entities.Common;

namespace Domain.Entities;

public class WorkLog : Entity
{
    public long IncidentId { get; set; }
    public long TechnicianId { get; set; }
    public decimal MinWorked { get; set; }
    public DateTime LogDate { get; set; } = DateTime.UtcNow;

    public User? Technician { get; set; }
    public Incident? Incident { get; set; }
}
