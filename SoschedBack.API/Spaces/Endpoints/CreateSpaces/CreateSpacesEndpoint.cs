using Microsoft.AspNetCore.Http.HttpResults;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.CreateSpaces;

public class CreateSpacesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new space")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string Name,
        string Domain
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string Domain
    );

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var space = new Space
        {
            Name = request.Name.Trim(),
            Domain = request.Domain.Trim()
        };

        await database.Spaces.AddAsync(space, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(space.Id, space.Name, space.Domain);
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}