using Microsoft.AspNetCore.Http.HttpResults;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.CreateTags;

public class CreateTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new tag")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string Name,
        string ShortName,
        string Color,
        int TagType
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string ShortName,
        string Color,
        int TagType
    );

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        SoschedBackDbContext database,
        ISpaceProvider spaceProvider,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var tag = new Tag
        {
            Name = request.Name.Trim(),
            ShortName = request.ShortName.Trim(),
            Color = request.Color,
            TagTypeId = request.TagType,
            SpaceId = spaceId
        };

        await database.Tags.AddAsync(tag, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(tag.Id, tag.Name, tag.ShortName, tag.Color, tag.TagTypeId);
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}