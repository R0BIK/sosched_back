using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Role : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int SpaceId { get; set; }
    
    public virtual ICollection<PermissionToRole> PermissionToRoles { get; set; } = new List<PermissionToRole>();
    
    public virtual Space Space { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}