namespace Suggestions.API.Contracts;

public class PagedList<TOutput>
{
    public List<TOutput>? Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PagedList() { }

    public PagedList(List<TOutput> items, int totalCount, int pageNumber, int pageSize)
    {
        this.Items = items;
        this.TotalCount = totalCount;
        this.Page = pageNumber;
        this.PageSize = pageSize;
    }
}