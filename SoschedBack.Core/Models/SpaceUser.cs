using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class SpaceUser : ISpaceEntity
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int RoleId { get; set; }
    
    public int SpaceEntityId { get; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual Role Role { get; set; } = null!;
}