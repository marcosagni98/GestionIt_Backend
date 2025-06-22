namespace Domain.Entities.Common;

public class Entity : EntityId
{
    public bool Active { get; set; } = true;

    public virtual void SoftDelete() => Active = false;
    public virtual void Activate() => Active = true;
}
