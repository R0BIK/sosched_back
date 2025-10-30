namespace SoschedBack.Common.Pagination;

public class PagedList<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public IEnumerable<T> Items { get; set; } = null!;

    public PagedList(
        IEnumerable<T> items, 
        int page, 
        int pageSize,
        int totalPages,
        int totalCount)
    {
        Items = items;

        Page = page;
        PageSize = pageSize;
        TotalPages = totalPages;
        TotalCount = totalCount;
    }

    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}