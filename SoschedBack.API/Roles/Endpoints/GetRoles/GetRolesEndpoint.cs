using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.GetRoles;

public class GetRolesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of roles.");

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest;

    public sealed record Response(
        int Id,
        string Name
    );

    private static async Task<IResult> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var roles = await database.Roles
            .ApplySorting(
                request.SortBy,
                request.Descending
            )
            .Select(role => new Response(
                role.Id,
                role.Name
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(roles);
        
        return Results.Ok(result);
    }
}