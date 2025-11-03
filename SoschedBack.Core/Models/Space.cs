using System.Collections;
using SoschedBack.Core.Models.Interfaces;

namespace SoschedBack.Core.Models;

public class Space : AuditableEntity, IEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public string Image { get; set; } = null!;
    
    public string? Website { get; set; }
    
    public string? Email { get; set; }
    
    public string? Address { get; set; }
    
    public string Domain { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    
    public virtual ICollection<TagType> TagTypes { get; set; } = new List<TagType>();
}