using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.GetRoleById;

public class RequestValidator : AbstractValidator<GetRoleByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db, ISpaceProvider spaceProvider)
    {
        var spaceId = spaceProvider.GetSpace();
        
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidSpaceEntityId<GetRoleByIdEndpoint.Request, Role>(db, spaceId);
            });
    }
}