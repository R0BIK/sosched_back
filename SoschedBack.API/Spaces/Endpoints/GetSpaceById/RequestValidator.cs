using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Models;
using SoschedBack.Storage;
using SoschedBack.Tags.Endpoints.GetTagById;

namespace SoschedBack.Spaces.Endpoints.GetSpaceById;

public class RequestValidator : AbstractValidator<GetSpaceByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db)
    {
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetSpaceByIdEndpoint.Request, Space>(db);
            });
    }
}