using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Event : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public int EventTypeId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int? CoordinatorId { get; set; }
    
    public string? Location { get; set; }
    
    public string? Description { get; set; }
    
    public int CreatorId { get; set; }
    
    public string Color { get; set; } = null!;
    
    public DateTime DateStart { get; set; }
    
    public DateTime DateEnd { get; set; }
    
    public int SpaceId { get; set; }
    
    public virtual EventType EventType { get; set; } = null!;
    
    public virtual User Creator { get; set; } = null!;
    
    public virtual User? Coordinator { get; set; }
    
    public virtual ICollection<EventToSpaceUser> EventToSpaceUsers { get; set; } = new List<EventToSpaceUser>();
    
    public virtual Space Space { get; set; } = null!;

}