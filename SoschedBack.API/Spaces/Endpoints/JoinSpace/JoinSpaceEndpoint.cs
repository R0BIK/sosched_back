using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.JoinSpace;

public class JoinSpaceEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/join", Handle)
        .WithSummary("Allows a user to join a public space by providing its domain and optionally, a password.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        string Domain,
        string? Password
    );

    private sealed record Response(
        int SpaceId,
        string SpaceDomain
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound, BadRequest<Result>>> Handle(
        Request request,
        IUserProvider userProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var user = userProvider.GetUser();
        var domain = request.Domain.Trim();
        var requestPassword = request.Password;

        // 1. Находим Space по Domain
        var space = await database.Spaces
            .FirstOrDefaultAsync(s => s.Domain == domain, cancellationToken);

        // 2. Проверка: Пользователь уже участник?
        var isAlreadyMember = await database.SpaceUsers
            .AsNoTracking()
            .AnyAsync(su => su.SpaceId == space.Id && su.UserId == user.Id, cancellationToken);
        
        if (isAlreadyMember)
        {
            var error = Error.From("User is already a member of this space.", "ALREADY_MEMBER");
            return TypedResults.BadRequest(Result.Failure(error));
        }
        
        if (!space.IsPublic)
        {
            var error = Error.From("Access to this space is restricted.", "SPACE_IS_PRIVATE");
            return TypedResults.BadRequest(Result.Failure(error));
        }
        
        // Определяем, отправил ли пользователь пароль (не null и не пустая строка)
        bool isPasswordProvidedByRequest = !string.IsNullOrEmpty(requestPassword);
        
        // Определяем, требует ли Space пароль
        bool isPasswordRequiredBySpace = !string.IsNullOrEmpty(space.Password);

        
        if (isPasswordRequiredBySpace)
        {
            // Сценарий A: Пароль требуется (публичный с паролем)
            if (space.Password != requestPassword) 
            {
                var error = Error.From("Invalid space password.", "INVALID_SPACE_PASSWORD");
                return TypedResults.BadRequest(Result.Failure(error));
            }
        }
        else // Пароль не требуется (Space публичный и без пароля)
        {
            // Сценарий B: Пароль не требуется, но пользователь его отправил
            if (isPasswordProvidedByRequest) 
            {
                var error = Error.From("This public space does not require a password. Please leave the password field empty.", "PASSWORD_NOT_REQUIRED");
                return TypedResults.BadRequest(Result.Failure(error));
            }
        }
        
        // --- КОНЕЦ ЛОГИКИ ДОСТУПА (Успешный вход) ---

        // 4. Добавление пользователя в Space
        var defaultRoleId = await database.Roles
            .Where(r => r.SpaceId == space.Id && r.Name == BaseRoleConstants.Guest)
            .Select(r => r.Id)
            .FirstAsync(cancellationToken);

        var spaceUser = new SpaceUser
        {
            SpaceId = space.Id,
            UserId = user.Id,
            RoleId = defaultRoleId
        };

        await database.SpaceUsers.AddAsync(spaceUser, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        var response = new Response(space.Id, space.Domain);
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}