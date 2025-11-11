using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class SpaceUser : IEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int SpaceId { get; set; }
    
    public int RoleId { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual Space Space { get; set; } = null!;
    
    public virtual Role Role { get; set; } = null!;
    
    public virtual ICollection<TagToSpaceUser> TagToSpaceUsers { get; set; } = new List<TagToSpaceUser>();
    
    public virtual ICollection<EventToSpaceUser> EventToUsers { get; set; } = new List<EventToSpaceUser>();
}