using System.Linq.Dynamic.Core;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Common.Responses;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.GetRoles;

public class GetRolesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of roles.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest, ISortRequest;

    private sealed record Response(
        int Id,
        string Name,
        int UsersCount
    ) : IUsersCountResponse;

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var rolesQuery = BuildSortedUsersCountQuery(request, spaceId, database);
        
        var roles = await rolesQuery
            .AsNoTracking()
            .Select(role => new Response(
                role.Id,
                role.Name,
                role.UsersCount
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(roles);
        
        return TypedResults.Ok(result);
    }
    
    private static IQueryable<Response> BuildSortedUsersCountQuery(
        Request request, 
        int spaceId,
        SoschedBackDbContext dbContext)
    {
        var query = dbContext.Roles
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId)
            .Select(role => new Response(
                role.Id,
                role.Name,
                dbContext.SpaceUsers.Count(tu => tu.RoleId == role.Id)
            ));

        return query.ApplySorting(request.SortBy, request.Descending);
    }
}