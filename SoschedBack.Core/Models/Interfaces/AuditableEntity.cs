namespace SoschedBack.Core.Models.Interfaces;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; } 
}