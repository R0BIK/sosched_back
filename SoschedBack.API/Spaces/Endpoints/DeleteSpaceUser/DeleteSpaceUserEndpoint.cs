using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.DeleteSpaceUser;

public class DeleteSpaceUsers : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapDelete("/leave/{userId:int}", Handle)
        .WithSummary("Removes a user from the current space (Leave Space / Kick User).")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int UserId
    );

    private static async Task<Results<Ok<Result<string>>, NotFound, BadRequest<Result>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        var userIdToRemove = request.UserId;

        // 1. Шукаємо запис про участь користувача в цьому спейсі
        var spaceUser = await database.SpaceUsers
            .FirstAsync(su => su.SpaceId == spaceId && su.UserId == userIdToRemove, cancellationToken);

        // (Опціонально) Тут можна додати перевірку:
        // - Якщо користувач видаляє сам себе -> дозволити (Вихід).
        // - Якщо користувач видаляє іншого -> перевірити права адміністратора (Кік).
        // - Перевірити, чи не є користувач єдиним адміністратором/власником (щоб не залишити спейс без адміна).

        // 2. Видаляємо зв'язок
        database.SpaceUsers.Remove(spaceUser);
        
        // 3. Також потрібно видалити цього користувача з усіх подій та тегів у цьому спейсі, 
        // щоб не залишилося "сирітських" записів у EventToSpaceUsers та TagToSpaceUsers.
        // Завдяки Cascade Delete в БД це може статися автоматично, але для надійності можна явно підвантажити або покластися на налаштування FK.
        // Якщо FK налаштовані на Cascade, видалення SpaceUser видалить і залежні записи.

        await database.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(Result.Success("User has left the space successfully."));
    }
}