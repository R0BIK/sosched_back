using FluentValidation;
using SoschedBack.Common.Extensions;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypes;

public class RequestValidator : AbstractValidator<GetTagTypesEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetTagTypesEndpoint.Request, AllowedSortField>();
    }
}