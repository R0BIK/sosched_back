using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;

namespace SoschedBack.Common.Extensions;

public static class PaginationDatabaseExtensions
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;

    public static async Task<PagedList<TResponse>> ToPagedListAsync<TRequest, TResponse>(
        this IQueryable<TResponse> query,
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IPagedRequest
    {
        var page = request.Page ?? DefaultPage;
        var pageSize = request.PageSize ?? DefaultPageSize;

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, IPagedRequest.MaxPageSize);

        var totalItems = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TResponse>(items, page, pageSize, totalPages, totalItems);
    }
}