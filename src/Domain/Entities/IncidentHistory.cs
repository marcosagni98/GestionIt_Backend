using Domain.Entities.Common;

namespace Domain.Entities;

public class IncidentHistory : EntityId
{
    public long IncidentId { get; set; }
    public Incident? Incident { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public long ChangedBy { get; set; }
    public User? ChangedByUser { get; set; }
    public string ResolutionDetails { get; set; } = string.Empty;
}
