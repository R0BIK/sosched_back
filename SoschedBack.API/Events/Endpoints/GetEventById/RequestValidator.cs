using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.GetTagById;

namespace SoschedBack.Events.Endpoints.GetEventById;

public class RequestValidator : AbstractValidator<GetTagByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidSpaceEntityId<GetTagByIdEndpoint.Request, Event>(db, spaceId);
            });
    }
}