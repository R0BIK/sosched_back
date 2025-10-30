using FluentValidation;
using SoschedBack.Common.Extensions;

namespace SoschedBack.Tags.Endpoints.GetTagById;

public class RequestValidator : AbstractValidator<GetTagByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .MustBeValidId();
    }
}