using SoschedBack.Core.Models;

namespace SoschedBack.Storage.Seeding;

public class SeedDataContainer
{
    public List<Space> Spaces { get; set; }
    public List<Permission> Permissions { get; set; }
    public List<EventType> EventTypes { get; set; }
    public List<Role> Roles { get; set; }
    public List<TagType> TagTypes { get; set; }
    public List<User> Users { get; set; }
    public List<Tag> Tags { get; set; }
    public List<Event> Events { get; set; }
    public List<PermissionToRole> PermissionToRoles { get; set; }
    public List<SpaceUser> SpaceUsers { get; set; }
    
    public List<TagToSpaceUser> TagToSpaceUsers { get; set; }
    public List<EventToSpaceUser> EventToSpaceUsers { get; set; }
}