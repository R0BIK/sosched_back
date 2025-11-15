using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.DeleteTag;

public class RequestValidator : AbstractValidator<DeleteTagByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();

        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                // Проверяем, что тег существует в текущем пространстве
                RuleFor(x => x.Id)
                    .MustBeValidSpaceEntityId<DeleteTagByIdEndpoint.Request, Core.Models.Tag>(db, spaceId);
            });
    }
}