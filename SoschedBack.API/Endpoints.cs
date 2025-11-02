using SoschedBack.Auth.Endpoints.Login;
using SoschedBack.Common;
using SoschedBack.Roles.Endpoints.CreateRoles;
using SoschedBack.Roles.Endpoints.GetRoleById;
using SoschedBack.Roles.Endpoints.GetRoles;
using SoschedBack.Tags.Endpoints.CreateTags;
using SoschedBack.Tags.Endpoints.GetTagById;
using SoschedBack.Tags.Endpoints.GetTags;
using SoschedBack.TagTypes.Endpoints.CreateTagTypes;
using SoschedBack.TagTypes.Endpoints.GetTagTypeById;
using SoschedBack.TagTypes.Endpoints.GetTagTypes;

namespace SoschedBack;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapTagEndpoints();
        app.MapTagTypeEndpoints();
        app.MapRoleEndpoints();
        app.MapAuthenticationEndpoints();
    }
    
    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Auth");

        endpoints.MapEndpoint<LoginEndpoint>();
    }
    
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/roles")
            .WithTags("Roles");

        endpoints.MapEndpoint<GetRolesEndpoint>();
        endpoints.MapEndpoint<GetRoleByIdEndpoint>();
        endpoints.MapEndpoint<CreateRolesEndpoint>();
    }

    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/tags")
            .WithTags("Tags");

        endpoints.MapEndpoint<GetTagsEndpoint>();
        endpoints.MapEndpoint<GetTagByIdEndpoint>();
        endpoints.MapEndpoint<CreateTagsEndpoint>();
    }

    public static void MapTagTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/tagTypes")
            .WithTags("TagTypes");

        endpoints.MapEndpoint<GetTagTypesEndpoint>();
        endpoints.MapEndpoint<GetTagTypeByIdEndpoint>();
        endpoints.MapEndpoint<CreateTagTypesEndpoint>();
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