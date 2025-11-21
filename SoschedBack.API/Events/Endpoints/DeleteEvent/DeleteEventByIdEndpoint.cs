using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Events.Endpoints.DeleteEvent;

public class DeleteEventByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:int}", Handle)
        .WithSummary("Deletes an event by id. Removes it from all users who participate in it.") // Обновлено
        .WithRequestValidation<Request>();

    public sealed record Request(int Id);

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext db,
        CancellationToken ct
    )
    {
        // Получаем событие
        var eventEntity = await db.Events // Смена сущности: Tags -> Events
            .FirstAsync(e => e.Id == request.Id, ct); // Смена: t.Id -> e.Id

        // Удаляем все связи Event -> SpaceUser
        var eventRelations = await db.EventToSpaceUsers // Смена таблицы: TagToSpaceUsers -> EventToSpaceUsers
            .Where(esu => esu.EventId == eventEntity.Id) // Смена ключа: tsu.TagId -> esu.EventId
            .ToListAsync(ct);

        if (eventRelations.Any())
        {
            db.EventToSpaceUsers.RemoveRange(eventRelations); // Удаляем связи
        }

        // Удаляем само событие
        db.Events.Remove(eventEntity); // Смена сущности: Tags -> Events

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Event deleted successfully")); // Обновлено сообщение
    }
}