using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTagById;

public class RequestValidator : AbstractValidator<GetTagByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db)
    {
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetTagByIdEndpoint.Request, Tag>(db);
            });
    }
}