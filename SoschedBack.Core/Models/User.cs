using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class User : AuditableEntity, ISpaceEntity
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string? Patronymic { get; set; }
    
    public DateOnly? Birthday { get; set; }
    
    public string IconPath { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public int SpaceId { get; set; }
    
    public virtual Space Space { get; set; } = null!;
    
    public virtual Role Role { get; set; } = null!;
}