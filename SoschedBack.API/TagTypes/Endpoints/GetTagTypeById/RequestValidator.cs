using FluentValidation;
using SoschedBack.Common.Extensions;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypeById;

public class RequestValidator : AbstractValidator<GetTagTypeByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .MustBeValidId();
    }
}