using FluentValidation;
using SoschedBack.Common.Extensions;
using SoschedBack.Tags.Endpoints.GetTagById;

namespace SoschedBack.Tags.Endpoints.GetTags;

public class RequestValidator : AbstractValidator<GetTagsEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetTagsEndpoint.Request, AllowedSortField>();
    }
}