namespace Domain.Entities.Common;

public class Entity : EntityId
{
    public bool Active { get; set; } = true;

    public virtual void Deactivate() => Active = false;
    public virtual void Activate() => Active = true;
}
