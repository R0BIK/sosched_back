using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Tags.Endpoints.GetTagById;

namespace SoschedBack.Roles.Endpoints.GetRoles;

public class RequestValidator : AbstractValidator<GetRolesEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetRolesEndpoint.Request, AllowedSortField>();
    }
}