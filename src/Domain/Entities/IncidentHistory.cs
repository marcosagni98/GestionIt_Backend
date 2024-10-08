using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class IncidentHistory : EntityId
{
    public long IncidentId { get; set; }
    public Status Status { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public long ChangedBy { get; set; }
    public string ResolutionDetails { get; set; } = string.Empty;

    public User? ChangedByUser { get; set; }
    public Incident? Incident { get; set; }
}
