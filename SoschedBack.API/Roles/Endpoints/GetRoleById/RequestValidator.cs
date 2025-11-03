using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.GetRoleById;

public class RequestValidator : AbstractValidator<GetRoleByIdEndpoint.Request>
{
    public RequestValidator(SoschedBackDbContext db)
    {
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetRoleByIdEndpoint.Request, Role>(db);
            });
    }
}