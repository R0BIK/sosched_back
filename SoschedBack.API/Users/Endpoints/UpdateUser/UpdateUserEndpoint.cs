using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;
using SoschedBack.Common.Http;

namespace SoschedBack.Users.Endpoints.UpdateUser;

public class UpdateUserEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPatch("/", Handle) 
            .WithSummary("Partially updates the current user profile.")
            .WithRequestValidation<Request>();
    }
    
    public sealed record Request(
        string? FirstName,
        string? LastName,
        string? Patronymic,
        DateOnly? Birthday,
        string? Email,
        string? Password
    );

    private sealed record Response(
        int Id,
        string Email
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound>> Handle(
        [FromBody] Request body,
        IPasswordHasher<User> passwordHasher,
        IUserProvider userProvider,
        SoschedBackDbContext db,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = userProvider.GetUser().Id;
        
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        UpdateUserEntity(user, body, passwordHasher);

        await db.SaveChangesAsync(cancellationToken);
        
        var response = new Response(user.Id, user.Email);
        return TypedResults.Ok(Result.Success(response));
    }
    
    /// <summary>
    /// Применяет частичные обновления к сущности User.
    /// </summary>
    private static void UpdateUserEntity(
        User user, 
        Request body, 
        IPasswordHasher<User> passwordHasher)
    {
        if (body.FirstName is not null)
        {
            user.FirstName = body.FirstName.Trim();
        }

        if (body.LastName is not null)
        {
            user.LastName = body.LastName.Trim();
        }
        
        if (body.Patronymic is not null)
        {
            // Patronymic может быть явно обнулен
            user.Patronymic = body.Patronymic.Trim(); 
        }

        if (body.Birthday.HasValue)
        {
            Console.WriteLine("DSADS: " + body.Birthday.Value);
            user.Birthday = body.Birthday.Value;
        }

        if (body.Email is not null)
        {
            // Email может требовать дополнительной проверки уникальности в валидаторе
            user.Email = body.Email.Trim().ToLower();
        }

        // Смена пароля: если Password передан, хешируем и сохраняем
        if (body.Password is not null)
        {
            // null! здесь используется, так как IPasswordHasher ожидает TUser, но в контексте хэширования
            // нам достаточно передать саму строку пароля.
            user.Password = passwordHasher.HashPassword(null!, body.Password);
        }
    }
}