using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Permissions.Endpoints.GetPermissionById;

public class GetPermissionByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a permission by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    private sealed record Response(
        int Id,
        string Name,
        string Description
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var permission = await database.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            permission.Id,
            permission.Name,
            permission.Description
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}