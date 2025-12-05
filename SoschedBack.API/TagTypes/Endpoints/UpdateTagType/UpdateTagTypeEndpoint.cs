using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.UpdateTagType;

public class UpdateTagTypeEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPatch("/{id:int}", Handle)
        .WithSummary("Partially updates an existing tag type.")
        .WithRequestValidation<RequestParameters>()
        .WithRequestValidation<RequestBody>();

    public sealed record RequestParameters(
        int Id
    );
    
    // RequestBody: Только Name, необязательный (nullable)
    public sealed record RequestBody(
        string? Name
    );
    
    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        // 1. Находим существующий тип тега по ID и SpaceId
        var tagType = await database.TagTypes
            .FirstAsync(t => t.Id == parameters.Id && t.SpaceId == spaceId, cancellationToken);

        UpdateTagTypeEntity(tagType, body);

        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(tagType.Id, tagType.Name);
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    private static void UpdateTagTypeEntity(
        TagType tagType, 
        RequestBody body)
    {
        if (body.Name is not null)
        {
            tagType.Name = body.Name.Trim();
        }
    }
}