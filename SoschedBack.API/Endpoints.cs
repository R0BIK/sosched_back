using SoschedBack.Auth.Endpoints.CheckAuth;
using SoschedBack.Auth.Endpoints.Login;
using SoschedBack.Auth.Endpoints.Logout;
using SoschedBack.Auth.Endpoints.Register;
using SoschedBack.Common;
using SoschedBack.Common.Filters;
using SoschedBack.Events.Endpoints.CreateEvents;
using SoschedBack.Events.Endpoints.GetEventById;
using SoschedBack.Events.Endpoints.GetEvents;
using SoschedBack.Permissions.Endpoints.GetPermissionById;
using SoschedBack.Permissions.Endpoints.GetPermissions;
using SoschedBack.Roles.Endpoints.CreateRoles;
using SoschedBack.Roles.Endpoints.GetRoleById;
using SoschedBack.Roles.Endpoints.GetRoles;
using SoschedBack.Search.SearchUsersAndTags;
using SoschedBack.Spaces.Endpoints.CreateSpaces;
using SoschedBack.Spaces.Endpoints.GetSpaceById;
using SoschedBack.Spaces.Endpoints.GetSpaces;
using SoschedBack.Tags.Endpoints.CreateTags;
using SoschedBack.Tags.Endpoints.DeleteTag;
using SoschedBack.Tags.Endpoints.GetTagById;
using SoschedBack.Tags.Endpoints.GetTags;
using SoschedBack.Tags.Endpoints.UpdateTag;
using SoschedBack.Tags.Endpoints.UpdateTagUsers;
using SoschedBack.TagTypes.Endpoints.CreateTagTypes;
using SoschedBack.TagTypes.Endpoints.DeleteTagType;
using SoschedBack.TagTypes.Endpoints.GetTagTypeById;
using SoschedBack.TagTypes.Endpoints.GetTagTypes;
using SoschedBack.Users.Endpoints.GetUserById;
using SoschedBack.Users.Endpoints.GetUsers;

namespace SoschedBack;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapTagEndpoints();
        app.MapTagTypeEndpoints();
        app.MapRoleEndpoints();
        app.MapAuthenticationEndpoints();
        app.MapSpaceEndpoints();
        app.MapUserEndpoints();
        app.MapPermissionsEndpoints();
        app.MapEventsEndpoints();
        app.MapSearchEndpoints();
    }
    
    private static void MapEventsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/events")
            .WithTags("Events");

        endpoints.MapEndpoint<CreateEventsEndpoint>();
        endpoints.MapEndpoint<GetEventsEndpoint>();
        endpoints.MapEndpoint<GetEventByIdEndpoint>();
    }
    
    private static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/search")
            .WithTags("Search");

        endpoints.MapEndpoint<SearchUsersAndTagsEndpoint>();
    }
    
    private static void MapPermissionsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/permissions")
            .WithTags("Permissions");

        endpoints.MapEndpoint<GetPermissionsEndpoint>();
        endpoints.MapEndpoint<GetPermissionByIdEndpoint>();
    }
    
    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Auth");

        endpoints.MapEndpoint<LoginEndpoint>();
        endpoints.MapEndpoint<Logout>();
        endpoints.MapEndpoint<RegisterEndpoint>();
        endpoints.MapEndpoint<CheckAuth>();
    }
    
    private static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/roles")
            .WithTags("Roles");

        endpoints.MapEndpoint<GetRolesEndpoint>();
        endpoints.MapEndpoint<GetRoleByIdEndpoint>();
        endpoints.MapEndpoint<CreateRolesEndpoint>();
    }
    
    private static void MapSpaceEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/spaces")
            .WithTags("Spaces")
            .RequireUserContext();

        endpoints.MapEndpoint<GetSpacesEndpoint>();
        endpoints.MapEndpoint<GetSpaceByIdEndpoint>();
        endpoints.MapEndpoint<CreateSpacesEndpoint>();
    }

    private static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/tags")
            .WithTags("Tags");

        endpoints.MapEndpoint<GetTagsEndpoint>();
        endpoints.MapEndpoint<GetTagByIdEndpoint>();
        endpoints.MapEndpoint<CreateTagsEndpoint>();
        endpoints.MapEndpoint<DeleteTagByIdEndpoint>();
        endpoints.MapEndpoint<UpdateTagUsersEndpoint>();
        endpoints.MapEndpoint<UpdateTagEndpoint>();
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/users")
            .WithTags("Users");

        endpoints.MapEndpoint<GetUsersEndpoint>();
        endpoints.MapEndpoint<GetUserByIdEndpoint>();
    }

    private static void MapTagTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapSpaceDomainGroup("/tagTypes")
            .WithTags("TagTypes");

        endpoints.MapEndpoint<GetTagTypesEndpoint>();
        endpoints.MapEndpoint<GetTagTypeByIdEndpoint>();
        endpoints.MapEndpoint<CreateTagTypesEndpoint>();
        endpoints.MapEndpoint<DeleteTagTypeByIdEndpoint>();
    }

    private static RouteGroupBuilder MapSpaceDomainGroup(this IEndpointRouteBuilder app, string route)
    {
        var group = app.MapGroup($"/{{spaceDomain}}{route}")
            .AddEndpointFilter<ExtractSpaceDomainFilter>()
            .RequireUserContext();
        
        return group;
    }
    
    private static RouteGroupBuilder RequireUserContext(this RouteGroupBuilder group)
    {
        return group.AddEndpointFilter<UserContextFilter>();
    }
    
    private static IEndpointConventionBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        var mapMethod = typeof(TEndpoint).GetMethod("Map", new[] { typeof(IEndpointRouteBuilder) });
        if (mapMethod == null)
            throw new InvalidOperationException($"Type {typeof(TEndpoint).Name} must have a static Map method with IEndpointRouteBuilder parameter.");

        var result = mapMethod.Invoke(null, new object[] { app });
        return result as IEndpointConventionBuilder ?? throw new InvalidOperationException("Map method must return IEndpointConventionBuilder");
    }
}