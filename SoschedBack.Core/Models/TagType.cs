using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class TagType : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Tag> Tags { get; set; } = null!;
}