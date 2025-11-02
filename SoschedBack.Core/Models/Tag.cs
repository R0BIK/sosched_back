using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Tag : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public int TagTypeId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string ShortName { get; set; } = null!;
    
    public string Color { get; set; } = null!;
    
    public int SpaceEntityId { get; }
    
    public virtual TagType TagType { get; set; } = null!;
    
    public virtual ICollection<TagToUser> TagToUsers { get; set; } = new List<TagToUser>();
    
    public virtual Space Space { get; set; } = null!;

}