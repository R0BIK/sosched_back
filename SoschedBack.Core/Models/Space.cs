using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Space : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public string Image { get; set; } = null!;
    
    public string? Website { get; set; }
    
    public string? Email { get; set; }
    
    public string? Address { get; set; }
}