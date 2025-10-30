using FluentValidation;

namespace SoschedBack.Common.Pagination.PagedRequest;

public class PagedRequestValidator<T> : AbstractValidator<T> 
    where T : IPagedRequest
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(IPagedRequest.MaxPageSize);
    }
}