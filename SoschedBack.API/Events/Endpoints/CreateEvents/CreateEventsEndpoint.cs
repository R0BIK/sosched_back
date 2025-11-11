using Microsoft.AspNetCore.Http.HttpResults;
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
        int EventTypeId,
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
        int EventTypeId,
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
        
        var myEvent = new Event
        {
            Name = request.Name.Trim(),
            Location = request.Location?.Trim(),
            Description = request.Description?.Trim(),
            Color = request.Color,
            DateStart = request.DateStart,
            DateEnd = request.DateEnd,
            EventTypeId = request.EventTypeId,
            CoordinatorId = request.CoordinatorId,
            CreatorId = user.Id,
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
                myEvent.DateStart,
                myEvent.DateEnd,
                myEvent.EventTypeId,
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
            myEvent.EventTypeId,
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

        DateTimeOffset currentDate = GetNextDate(myEvent.DateStart, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
        
        while (currentDate <= repeatInfo.RepeatEnd)
        {
            Event nextEvent = myEvent;
            
            nextEvent.DateStart = GetNextDate(nextEvent.DateStart, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
            nextEvent.DateEnd = GetNextDate(nextEvent.DateEnd, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
            
            currentDate = GetNextDate(currentDate, repeatInfo.RepeatNumber, repeatInfo.RepeatType);
            
            repeats.Add(nextEvent);
        }
        
        return repeats;
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