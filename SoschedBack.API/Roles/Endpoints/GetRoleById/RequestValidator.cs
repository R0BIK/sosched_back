using FluentValidation;
using SoschedBack.Common.Extensions;

namespace SoschedBack.Roles.Endpoints.GetRoleById;

public class RequestValidator : AbstractValidator<GetRoleByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .MustBeValidId();
    }
}