using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Event : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Location { get; set; }
    
    public string? Description { get; set; }
    
    public int CreatorId { get; set; }
    
    public int EventDateId { get; set; }
    
    public string Color { get; set; } = null!;
    
    public DateTime DateStart { get; set; }
    
    public DateTime DateEnd { get; set; }
}