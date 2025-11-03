using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class EventType : AuditableEntity, IEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}