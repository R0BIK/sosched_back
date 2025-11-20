using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.UpdateTag;

public class UpdateTagEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPatch("/{tagId:int}", Handle)
        .WithSummary("Updates an existing tag")
        .WithRequestValidation<RequestParameters>()
        .WithRequestValidation<RequestBody>();

    public sealed record RequestParameters(
        int TagId
    );
    
    public sealed record RequestBody(
        string? Name,
        string? ShortName,
        string? Color,
        int? TagType
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string ShortName,
        string Color,
        int TagType
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        SoschedBackDbContext database,
        ISpaceProvider spaceProvider,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var tag = await database.Tags
            .AsNoTracking()
            .FirstAsync(
            t => t.Id == parameters.TagId && t.SpaceId == spaceId, 
            cancellationToken
        );

        UpdateTagEntity(tag, body);

        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(tag.Id, tag.Name, tag.ShortName, tag.Color, tag.TagTypeId);
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    private static void UpdateTagEntity(Tag tag, RequestBody body)
    {
        if (body.Name is not null)
        {
            tag.Name = body.Name.Trim();
        }

        if (body.ShortName is not null)
        {
            tag.ShortName = body.ShortName.Trim();
        }

        if (body.Color is not null)
        {
            tag.Color = body.Color;
        }

        if (body.TagType is not null)
        {
            tag.TagTypeId = body.TagType.Value;
        }
    }
}