using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Permission : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public virtual ICollection<PermissionToRole> PermissionToRoles { get; set; } = new List<PermissionToRole>();

}