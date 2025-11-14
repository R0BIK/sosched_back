using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.DeleteTagType;

public class RequestValidator : AbstractValidator<DeleteTagTypeByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();

        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidSpaceEntityId<DeleteTagTypeByIdEndpoint.Request, TagType>(db, spaceId);

                RuleFor(x => x.Id)
                    .MustAsync(async (id, ct) =>
                    {
                        return !await db.Tags
                            .AnyAsync(t => t.TagTypeId == id && t.SpaceId == spaceId, ct);
                    })
                    .WithMessage("Cannot delete TagType that is used by existing tags.");
            });
    }
}