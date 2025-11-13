using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.CreateTagTypes;

public class CreateTagTypesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new tag type.")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string Name
    );
    
    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var tagType = new TagType
        {
            Name = request.Name.Trim(),
            SpaceId = spaceId
        };

        await database.TagTypes.AddAsync(tagType, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(tagType.Id, tagType.Name);
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}