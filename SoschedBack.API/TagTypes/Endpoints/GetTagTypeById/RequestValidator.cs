using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypeById;

public class RequestValidator : AbstractValidator<GetTagTypeByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidSpaceEntityId<GetTagTypeByIdEndpoint.Request, TagType>(db, spaceId);
            });
    }
}