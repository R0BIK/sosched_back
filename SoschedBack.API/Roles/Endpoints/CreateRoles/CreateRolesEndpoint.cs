using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.CreateRoles;

public class CreateRolesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new role")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string Name
    );
    
    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<IResult> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        if (await IsRoleNameExists(request.Name, database, cancellationToken)) 
            return Results.BadRequest($"Role with '{request.Name}' name already exists");

        var role = new Role
        {
            Name = request.Name.Trim()
        };

        database.Roles.Add(role);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(role.Id, role.Name);

        var result = Result.Success(response);
        
        return Results.Ok(result);
    }

    private static async Task<bool> IsRoleNameExists(
        string name,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        return await database.Roles.AnyAsync(r => r.Name == name, cancellationToken);
    }
}