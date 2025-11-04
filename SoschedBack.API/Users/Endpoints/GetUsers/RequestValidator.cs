using FluentValidation;
using SoschedBack.Common.Extensions;

namespace SoschedBack.Users.Endpoints.GetUsers;

public class RequestValidator : AbstractValidator<GetUsersEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetUsersEndpoint.Request, AllowedSortField>();
    }
}