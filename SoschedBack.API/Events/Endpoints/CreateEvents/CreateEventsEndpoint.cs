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
        DateOnly RepeatEnd
    );
    
    public sealed record Request(
        string Name,
        string? Location,
        string? Description,
        string Color,
        DateOnly Date,
        TimeSpan TimeStart,  
        TimeSpan TimeEnd,
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
        DateOnly Date,
        TimeSpan TimeStart,  
        TimeSpan TimeEnd,
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
                .FirstAsync(
                    su => su.SpaceId == spaceId && su.UserId == request.CoordinatorId.Value, 
                    cancellationToken
                );

            coordinatorSpaceUserId = coordinatorSpaceUserEntity?.Id;
        }
        
        var datePart = request.Date.ToDateTime(TimeOnly.MinValue); 
        var dateStartCombined = datePart.Add(request.TimeStart);
        var dateEndCombined = datePart.Add(request.TimeEnd);
        
        var myEvent = new Event
        {
            Name = request.Name.Trim(),
            Location = request.Location?.Trim(),
            Description = request.Description?.Trim(),
            Color = request.Color,
            DateStart = dateStartCombined,
            DateEnd = dateEndCombined, 
            CoordinatorId = coordinatorSpaceUserId, 
            CreatorId = creatorSpaceUser.Id, 
            SpaceId = spaceId,
        };

        if (request.RepeatInfo is null)
        {
            await database.Events.AddAsync(myEvent, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        
            var response = new Response(
                myEvent.Id,
                myEvent.Name,
                myEvent.Location,
                myEvent.Description,
                myEvent.Color,
                DateOnly.FromDateTime(myEvent.DateStart.Date),
                myEvent.DateStart.TimeOfDay,
                myEvent.DateEnd.TimeOfDay,
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
            DateOnly.FromDateTime(myEvent.DateStart.Date),
            myEvent.DateStart.TimeOfDay,
            myEvent.DateEnd.TimeOfDay,
            myEvent.CoordinatorId,
            repeats.Count
        );
        
        if (!request.Confirmed)
            return TypedResults.Ok(Result.Success(preview));
        
        await database.Events.AddRangeAsync(repeats, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(Result.Success(preview));
    }

    private static List<Event> GenerateRepeats(RepeatInfo repeatInfo, Event myEvent)
    {
        var repeats = new List<Event>();

        var baseEvent = CloneEventData(myEvent);

        var repeatEndDateTime = repeatInfo.RepeatEnd
            .ToDateTime(TimeOnly.MaxValue); 
            
        var repeatEndOffset = new DateTimeOffset(repeatEndDateTime, myEvent.DateStart.Offset);
        
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