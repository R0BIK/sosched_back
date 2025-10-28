using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Tag : AuditableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
}