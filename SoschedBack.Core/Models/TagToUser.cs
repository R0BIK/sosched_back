namespace SoschedBack.Core.Models;

public class TagToUser
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int TagId { get; set; }

    public virtual User User { get; set; } = null!;
    
    public virtual Tag Tag { get; set; } = null!;
}