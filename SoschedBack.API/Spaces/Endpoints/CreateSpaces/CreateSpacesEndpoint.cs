using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
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
        IUserProvider userProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var user = userProvider.GetUser();
        
        var space = new Space
        {
            Name = request.Name.Trim(),
            Domain = request.Domain.Trim()
        };

        await database.Spaces.AddAsync(space, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        await CreateDefaultRolesAsync(space.Id, database, cancellationToken);
        await AddCreatorAsAdminAsync(space.Id, user.Id, database, cancellationToken);
        
        var response = new Response(space.Id, space.Name, space.Domain);
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    private static async Task CreateDefaultRolesAsync(
        int spaceId,
        SoschedBackDbContext database,
        CancellationToken ct)
    {
        var defaultRoles = new List<Role>
        {
            new Role
            {
                Name = BaseRoleConstants.Admin,
                SpaceId = spaceId
            },
            new Role
            {
                Name = BaseRoleConstants.Guest,
                SpaceId = spaceId
            }
        };
        
        //TODO: Add permissions

        await database.Roles.AddRangeAsync(defaultRoles, ct);
        await database.SaveChangesAsync(ct);
    }
    
    private static async Task AddCreatorAsAdminAsync(
        int spaceId,
        int creatorId,
        SoschedBackDbContext database,
        CancellationToken ct)
    {
        var adminRole = await database.Roles
            .AsNoTracking()
            .FirstAsync(r => r.SpaceId == spaceId && r.Name == BaseRoleConstants.Admin, ct);

        var spaceUser = new SpaceUser
        {
            SpaceId = spaceId,
            UserId = creatorId,
            RoleId = adminRole.Id
        };

        await database.SpaceUsers.AddAsync(spaceUser, ct);
        await database.SaveChangesAsync(ct);
    }
}