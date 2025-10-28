namespace SoschedBack.Core.Models;

public class PermissionToRole
{
    public int Id { get; set; }

    public int PermissionId { get; set; }
    
    public int RoleId { get; set; }
}