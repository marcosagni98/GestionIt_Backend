using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Common;

public class EntityId 
{
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public long Id { get; set; }
}
