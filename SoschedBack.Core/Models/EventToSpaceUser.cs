using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class EventToSpaceUser : IEntity
{
    public int Id { get; set; }
    
    public int EventId { get; set; }
    
    public int SpaceUserId { get; set; }
    
    public virtual Event Event { get; set; }
    
    public virtual SpaceUser SpaceUser { get; set; }
}