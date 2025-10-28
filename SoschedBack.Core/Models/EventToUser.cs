namespace SoschedBack.Core.Models;

public class EventToUser
{
    public int Id { get; set; }
    
    public int EventId { get; set; }
    
    public int UserId { get; set; }
}