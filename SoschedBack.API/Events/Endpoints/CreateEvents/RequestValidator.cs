using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.CreateTags;

namespace SoschedBack.Events.Endpoints.CreateEvents;

public class RequestValidator : AbstractValidator<CreateEventsEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext database, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();

        RuleFor(x => x.Name)
            .MustBeValidTitle();

        RuleFor(x => x.Location)
            .MustBeValidOptionalString();

        RuleFor(x => x.Description)
            .MustBeValidOptionalDescription();

        RuleFor(x => x.Color)
            .MustBeValidString();
        
        RuleFor(x => x.CoordinatorId)
            .MustBeValidOptionalId()
            .DependentRules(() =>
            {
                RuleFor(x => x.CoordinatorId)
                    .MustBeValidOptionalSpaceEntityId<CreateEventsEndpoint.Request, SpaceUser>(database, spaceId)
                    .When(x => x.CoordinatorId.HasValue);
            });

        RuleFor(r => r.Date)
            .MustBeValidDate();

        RuleFor(r => r.TimeStart)
            .LessThan(x => x.TimeEnd)
            .WithMessage("DateStart must be earlier than DateEnd.");
        
        RuleFor(r => r.TimeEnd)
            .GreaterThan(x => x.TimeStart)
            .WithMessage("DateEnd must be later than DateStart.");
        
        RuleFor(x => x.RepeatInfo)
            .Must(repeat =>
            {
                if (repeat is null) return true;
                return repeat.RepeatNumber > 0
                       && !string.IsNullOrWhiteSpace(repeat.RepeatType)
                       && repeat.RepeatEnd != default;
            })
            .WithMessage("If RepeatInfo is provided, all fields must be filled.");

        RuleFor(x => x.RepeatInfo!.RepeatNumber)
            .GreaterThan(0)
            .WithMessage("RepeatNumber must be greater than 0.")
            .When(x => x.RepeatInfo != null);

        RuleFor(x => x.RepeatInfo!.RepeatType)
            .MustBeValidRepeatType()
            .When(x => x.RepeatInfo != null);

        RuleFor(x => x.RepeatInfo!.RepeatEnd)
            .GreaterThan(x => x.Date)
            .WithMessage("RepeatEnd must be after DateEnd.")
            .When(x => x.RepeatInfo != null);

        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                if (!IsSuccessfulValidation(request)) return;
                
                var name = request.Name.Trim();
                var location = request.Location?.Trim();
                var coordinatorId = request.CoordinatorId;
                
                var datePart = request.Date.ToDateTime(TimeOnly.MinValue); 
                var dateStartCombined = datePart.Add(request.TimeStart);
                var dateEndCombined = datePart.Add(request.TimeEnd);

                var exists = await database.Events
                    .AsNoTracking()
                    .AnyAsync(e =>
                            e.SpaceId == spaceId &&
                            e.Name == name &&
                            e.Location == location &&
                            e.DateStart == dateStartCombined &&
                            e.DateEnd == dateEndCombined &&
                            e.CoordinatorId == coordinatorId,
                        cancellationToken);

                if (exists)
                {
                    context.AddFailure("An identical event already exists.");
                }
            });
    }

    private static bool IsSuccessfulValidation(CreateEventsEndpoint.Request request)
    {
        var validator = new InlineValidator<CreateEventsEndpoint.Request>();

        validator.RuleFor(x => x.Name).MustBeValidTitle();
        validator.RuleFor(x => x.Color).MustBeValidString();

        validator.RuleFor(x => x.TimeStart)
            .LessThan(x => x.TimeEnd);

        validator.RuleFor(x => x.TimeEnd)
            .GreaterThan(x => x.TimeStart);

        validator.RuleFor(x => x.CoordinatorId)
            .MustBeValidOptionalId();

        validator.RuleFor(x => x.RepeatInfo)
            .Must(repeat =>
            {
                if (repeat is null) return true;

                return repeat.RepeatNumber > 0
                       && !string.IsNullOrWhiteSpace(repeat.RepeatType)
                       && repeat.RepeatEnd != default;
            });
        
        var result = validator.Validate(request);
        return result.IsValid;
    }
}