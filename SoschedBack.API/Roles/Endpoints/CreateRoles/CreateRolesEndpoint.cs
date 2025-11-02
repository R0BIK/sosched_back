using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Auth.Endpoints.Login;
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

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var role = new Role
        {
            Name = request.Name.Trim()
        };

        database.Roles.Add(role);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(role.Id, role.Name);

        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
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