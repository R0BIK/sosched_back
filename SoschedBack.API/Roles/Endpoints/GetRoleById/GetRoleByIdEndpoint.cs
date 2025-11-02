using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.GetRoleById;

public class GetRoleByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a role by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    public sealed record Response(
        int Id,
        string Name
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var role = await database.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            role.Id,
            role.Name
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}