using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypeById;

public class RequestValidator : AbstractValidator<GetTagTypeByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db)
    {
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetTagTypeByIdEndpoint.Request, TagType>(db);
            });
    }
}