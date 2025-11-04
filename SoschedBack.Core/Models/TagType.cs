using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class TagType : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int SpaceId { get; set; }
    
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    
    public virtual Space Space { get; set; } = null!;

}