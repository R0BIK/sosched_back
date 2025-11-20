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
        DateOnly? Date,
        TimeSpan? TimeStart,
        TimeSpan? TimeEnd,
        int? CoordinatorId
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string? Location,
        string? Description,
        string Color,
        DateOnly Date,
        TimeSpan TimeStart,
        TimeSpan TimeEnd,
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
        
        var eventEntity = await database.Events
            .FirstAsync(
                e => e.Id == parameters.EventId && e.SpaceId == spaceId, 
                cancellationToken
            );

        int? resolvedCoordinatorSpaceUserId = null;
        if (body.CoordinatorId.HasValue)
        {
            var coordinatorSpaceUserEntity = await database.SpaceUsers
                .AsNoTracking()
                .FirstAsync(
                    su => su.SpaceId == spaceId && su.UserId == body.CoordinatorId.Value, 
                    cancellationToken
                );
            
            resolvedCoordinatorSpaceUserId = coordinatorSpaceUserEntity?.Id;
        }

        UpdateEventEntity(eventEntity, body, resolvedCoordinatorSpaceUserId);

        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(
            eventEntity.Id,
            eventEntity.Name,
            eventEntity.Location,
            eventEntity.Description,
            eventEntity.Color,
            DateOnly.FromDateTime(eventEntity.DateStart.Date),
            eventEntity.DateStart.TimeOfDay,
            eventEntity.DateEnd.TimeOfDay,
            eventEntity.CoordinatorId
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
    
    private static void UpdateEventEntity(
        Event eventEntity, 
        RequestBody body, 
        int? resolvedCoordinatorSpaceUserId)
    {
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
        
        if (body.CoordinatorId.HasValue)
        {
            eventEntity.CoordinatorId = resolvedCoordinatorSpaceUserId;
        }
        
        
        // Проверяем, изменил ли пользователь что-либо связанное с датой/временем
        if (body.Date.HasValue || body.TimeStart.HasValue || body.TimeEnd.HasValue)
        {
            // Определяем дату: используем новое значение ИЛИ существующую дату
            var datePart = body.Date ?? DateOnly.FromDateTime(eventEntity.DateStart.Date);
            
            // Определяем время начала: используем новое значение ИЛИ существующее время
            var timeStartPart = body.TimeStart ?? eventEntity.DateStart.TimeOfDay;
            
            // Определяем время конца: используем новое значение ИЛИ существующее время
            var timeEndPart = body.TimeEnd ?? eventEntity.DateEnd.TimeOfDay;

            // Объединяем компоненты
            var newBaseDateTime = datePart.ToDateTime(TimeOnly.MinValue);
            
            eventEntity.DateStart = newBaseDateTime.Add(timeStartPart);
            eventEntity.DateEnd = newBaseDateTime.Add(timeEndPart);
        }
    }
}