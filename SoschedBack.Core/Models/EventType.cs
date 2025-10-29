using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class EventType : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
}