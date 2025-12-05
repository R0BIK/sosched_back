using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Events.Endpoints.UpdateEvent;

public class UpdateEventEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPatch("/{eventId:int}", Handle)
        .WithSummary("Partially updates an existing event in the current space.")
        .WithRequestValidation<RequestParameters>()
        .WithRequestValidation<RequestBody>();

    public sealed record RequestParameters(
        int EventId
    );
    
    public sealed record RequestBody(
        string? Name,
        string? Location,
        string? Description,
        string? Color,
        DateTimeOffset? DateStart,
        DateTimeOffset? DateEnd, 
        int? CoordinatorId
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string? Location,
        string? Description,
        string Color,
        DateTimeOffset DateStart,
        DateTimeOffset DateEnd,
        int? CoordinatorId
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        SoschedBackDbContext database,
        ISpaceProvider spaceProvider,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        // 1. Находим существующее событие
        var eventEntity = await database.Events
            .FirstAsync(
                e => e.Id == parameters.EventId && e.SpaceId == spaceId, 
                cancellationToken
            );

        // Сохраняем оригинальный SpaceUser ID координатора для логики удаления
        int? originalCoordinatorSpaceUserId = eventEntity.CoordinatorId;

        // 2. Если передан новый CoordinatorId, находим его SpaceUser ID
        int? newCoordinatorSpaceUserId = null;
        if (body.CoordinatorId.HasValue)
        {
            // Ищем SpaceUser в текущем SpaceId по Global UserId
            var coordinatorSpaceUserEntity = await database.SpaceUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    su => su.SpaceId == spaceId && su.UserId == body.CoordinatorId.Value, 
                    cancellationToken
                );

            newCoordinatorSpaceUserId = coordinatorSpaceUserEntity?.Id;
        }

        // 3. Обновляем сущность (eventEntity.CoordinatorId теперь содержит новое значение)
        UpdateEventEntity(eventEntity, body, newCoordinatorSpaceUserId);
        
        // --- ЛОГИКА УПРАВЛЕНИЯ УЧАСТИЕМ КООРДИНАТОРА ---
        
        // 4. Проверяем, изменился ли координатор (или был сброшен/установлен)
        if (originalCoordinatorSpaceUserId != eventEntity.CoordinatorId)
        {
            await PerformCoordinatorParticipationUpdateAsync(
                database,
                eventEntity.Id,
                originalCoordinatorSpaceUserId,
                eventEntity.CoordinatorId, // eventEntity.CoordinatorId содержит новое значение
                cancellationToken);
        }
        
        // 5. Сохраняем все изменения в базе данных (обновление Event и EventToSpaceUsers)
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(
            eventEntity.Id,
            eventEntity.Name,
            eventEntity.Location,
            eventEntity.Description,
            eventEntity.Color,
            eventEntity.DateStart, // Прямой возврат
            eventEntity.DateEnd,   // Прямой возврат
            eventEntity.CoordinatorId
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    /// <summary>
    /// Применяет частичные обновления к сущности Event.
    /// </summary>
    private static void UpdateEventEntity(
        Event eventEntity, 
        RequestBody body, 
        int? resolvedCoordinatorSpaceUserId)
    {
        // 1. Простые поля
        if (body.Name is not null)
        {
            eventEntity.Name = body.Name.Trim();
        }

        if (body.Location is not null)
        {
            eventEntity.Location = body.Location.Trim();
        }

        if (body.Description is not null)
        {
            eventEntity.Description = body.Description.Trim();
        }

        if (body.Color is not null)
        {
            eventEntity.Color = body.Color;
        }
        
        eventEntity.CoordinatorId = resolvedCoordinatorSpaceUserId;
        
        if (body.DateStart.HasValue)
        {
            eventEntity.DateStart = body.DateStart.Value;
        }
        
        if (body.DateEnd.HasValue)
        {
            eventEntity.DateEnd = body.DateEnd.Value;
        }
    }
    
    /// <summary>
    /// Управляет добавлением и удалением координатора из EventToSpaceUsers.
    /// Вызывается только если старый и новый ID координатора не совпадают.
    /// </summary>
    private static async Task PerformCoordinatorParticipationUpdateAsync(
        SoschedBackDbContext database,
        int eventId,
        int? oldCoordinatorSpaceUserId,
        int? newCoordinatorSpaceUserId,
        CancellationToken cancellationToken)
    {
        // A. Удаляем старое участие (если координатор существовал)
        if (oldCoordinatorSpaceUserId.HasValue)
        {
            var oldParticipation = await database.EventToSpaceUsers
                .Where(esu => esu.EventId == eventId && esu.SpaceUserId == oldCoordinatorSpaceUserId.Value)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (oldParticipation != null)
            {
                database.EventToSpaceUsers.Remove(oldParticipation);
            }
        }
        
        // B. Добавляем новое участие (если новый координатор установлен)
        if (newCoordinatorSpaceUserId.HasValue) 
        {
            // Проверяем, что пользователь не был добавлен ранее (хотя удаление должно было это гарантировать)
            var isAlreadyParticipating = await database.EventToSpaceUsers
                .AnyAsync(esu => esu.EventId == eventId && esu.SpaceUserId == newCoordinatorSpaceUserId.Value, cancellationToken);

            if (!isAlreadyParticipating)
            {
                var newParticipation = new EventToSpaceUser
                {
                    EventId = eventId,
                    SpaceUserId = newCoordinatorSpaceUserId.Value
                };
                database.EventToSpaceUsers.Add(newParticipation);
            }
        }
    }
}