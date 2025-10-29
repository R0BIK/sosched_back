using SoschedBack.Common;
using SoschedBack.Tags.Endpoints.CreateTags;
using SoschedBack.Tags.Endpoints.GetTags;

namespace SoschedBack;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapTagEndpoints();
    }

    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/tags")
            .WithTags("Tags");

        endpoints.MapEndpoint<GetTagsEndpoint>();
        endpoints.MapEndpoint<CreateTagsEndpoint>();
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