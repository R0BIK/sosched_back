using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Requests;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Events.Endpoints.CreateEvents;

public class CreateEventsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new event")
        .WithRequestValidation<Request>();

    public sealed record RepeatInfo(
        int RepeatNumber,
        string RepeatType,
        DateTimeOffset RepeatEnd
    );
    
    public sealed record Request(
        string Name,
        string? Location,
        string? Description,
        string Color,
        DateTimeOffset DateStart,
        DateTimeOffset DateEnd, 
        int? CoordinatorId,
        RepeatInfo? RepeatInfo,
        bool Confirmed = false
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string? Location,
        string? Description,
        string Color,
        DateTimeOffset DateStart,
        DateTimeOffset DateEnd,  
        int? CoordinatorId,
        int? RepeatsCount
    );

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        SoschedBackDbContext database,
        ISpaceProvider spaceProvider,
        IUserProvider userProvider,
        CancellationToken cancellationToken
    )
    {
        var spaceId = spaceProvider.GetSpace();
        var user = userProvider.GetUser();
        
        var creatorSpaceUser = await database.SpaceUsers
            .AsNoTracking()
            .FirstAsync(
                su => su.SpaceId == spaceId && su.UserId == user.Id, 
                cancellationToken
            );
        
        int? coordinatorSpaceUserId = null;

        if (request.CoordinatorId.HasValue)
        {
            var coordinatorSpaceUserEntity = await database.SpaceUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    su => su.SpaceId == spaceId && su.UserId == request.CoordinatorId.Value, 
                    cancellationToken
                );

            coordinatorSpaceUserId = coordinatorSpaceUserEntity?.Id;
        }
        
        var myEvent = new Event
        {
            Name = request.Name.Trim(),
            Location = request.Location?.Trim(),
            Description = request.Description?.Trim(),
            Color = request.Color,
            DateStart = request.DateStart,
            DateEnd = request.DateEnd,   
            CoordinatorId = coordinatorSpaceUserId, 
            CreatorId = creatorSpaceUser.Id, 
            SpaceId = spaceId,
        };

        if (request.RepeatInfo is null)
        {
            await database.Events.AddAsync(myEvent, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            
            if (coordinatorSpaceUserId.HasValue)
            {
                await AddCoordinatorParticipation(database, myEvent.Id, coordinatorSpaceUserId.Value, cancellationToken);
                await database.SaveChangesAsync(cancellationToken); 
            }
        
            var response = new Response(
                myEvent.Id,
                myEvent.Name,
                myEvent.Location,
                myEvent.Description,
                myEvent.Color,
                myEvent.DateStart,
                myEvent.DateEnd,
                myEvent.CoordinatorId,
                null
            );
        
            var result = Result.Success(response);
        
            return TypedResults.Ok(result);
        }

        var repeats = GenerateRepeats(request.RepeatInfo, myEvent);
        
        var preview = new Response(
            myEvent.Id,
            myEvent.Name,
            myEvent.Location,
            myEvent.Description,
            myEvent.Color,
            myEvent.DateStart,
            myEvent.DateEnd,
            myEvent.CoordinatorId,
            repeats.Count
        );
        
        if (!request.Confirmed)
            return TypedResults.Ok(Result.Success(preview));
        
        await database.Events.AddRangeAsync(repeats, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        var eventIds = repeats.Select(e => e.Id).ToList();
        
        if (coordinatorSpaceUserId.HasValue)
        {
            await AddCoordinatorParticipationToMultipleEvents(database, eventIds, coordinatorSpaceUserId.Value, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        }

        return TypedResults.Ok(Result.Success(preview));
    }
    
    private static async Task AddCoordinatorParticipation(
        SoschedBackDbContext database, 
        int eventId, 
        int spaceUserId, 
        CancellationToken cancellationToken)
    {
        var participation = new EventToSpaceUser
        {
            EventId = eventId,
            SpaceUserId = spaceUserId
        };
        await database.EventToSpaceUsers.AddAsync(participation, cancellationToken);
    }
    
    private static async Task AddCoordinatorParticipationToMultipleEvents(
        SoschedBackDbContext database, 
        IEnumerable<int> eventIds, 
        int spaceUserId, 
        CancellationToken cancellationToken)
    {
        var participations = eventIds.Select(id => new EventToSpaceUser
        {
            EventId = id,
            SpaceUserId = spaceUserId
        }).ToList();
        await database.EventToSpaceUsers.AddRangeAsync(participations, cancellationToken);
    }

    private static List<Event> GenerateRepeats(RepeatInfo repeatInfo, Event myEvent)
    {
        var repeats = new List<Event>();

        var baseEvent = CloneEventData(myEvent);

        // ИСПРАВЛЕНО: Сравнение DateTimeOffset с DateTimeOffset
        DateTimeOffset repeatEndOffset = repeatInfo.RepeatEnd; 
        
        DateTimeOffset currentDate = GetNextDate(baseEvent.DateStart, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
        
        while (currentDate <= repeatEndOffset)
        {
            var nextEvent = CloneEventData(baseEvent); 
            
            nextEvent.DateStart = GetNextDate(baseEvent.DateStart, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
            nextEvent.DateEnd = GetNextDate(baseEvent.DateEnd, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
            
            baseEvent.DateStart = nextEvent.DateStart;
            baseEvent.DateEnd = nextEvent.DateEnd;

            currentDate = nextEvent.DateStart;
            
            repeats.Add(nextEvent);
        }
        
        return repeats;
    }
    
    private static Event CloneEventData(Event source)
    {
        return new Event
        {
            Name = source.Name,
            Location = source.Location,
            Description = source.Description,
            Color = source.Color,
            CoordinatorId = source.CoordinatorId,
            CreatorId = source.CreatorId,
            SpaceId = source.SpaceId,
            DateStart = source.DateStart,
            DateEnd = source.DateEnd
        };
    }

    private static DateTimeOffset GetNextDate(DateTimeOffset date, int number, string type)
    {
        return type switch
        {
            RepeatConstants.Day.RepeatType => date.AddDays(number),
            RepeatConstants.Week.RepeatType => date.AddDays(number * 7),
            RepeatConstants.Month.RepeatType => date.AddMonths(number),
            _ => throw new ArgumentOutOfRangeException($"Unknown repeat type: {type}")
        };
    }
}