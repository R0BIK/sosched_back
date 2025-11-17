using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Permissions.Endpoints.GetPermissions;

public class GetPermissionsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of permissions.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10
    ) : IPagedRequest;
    
    private sealed record Response(
        int Id,
        string Name,
        string Description
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var permissions = await database.Permissions
            .AsNoTracking()
            .Select(permission => new Response(
                permission.Id,
                permission.Name,
                permission.Description
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(permissions);
        
        return TypedResults.Ok(result);
    }
}