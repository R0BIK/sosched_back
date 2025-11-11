using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Tag : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public int TagTypeId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string ShortName { get; set; } = null!;
    
    public string Color { get; set; } = null!;
    
    public int SpaceId { get; set; }
    
    public virtual TagType TagType { get; set; } = null!;
    
    public virtual ICollection<TagToSpaceUser> TagToSpaceUsers { get; set; } = new List<TagToSpaceUser>();
    
    public virtual Space Space { get; set; } = null!;

}