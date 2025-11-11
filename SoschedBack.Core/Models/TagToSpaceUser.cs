using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class TagToSpaceUser : IEntity
{
    public int Id { get; set; }
    
    public int SpaceUserId { get; set; }
    
    public int TagId { get; set; }

    public virtual SpaceUser SpaceUser { get; set; } = null!;
    
    public virtual Tag Tag { get; set; } = null!;
}