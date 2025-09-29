using System.Text.Json.Serialization;

namespace Suggestions.API.Contracts;

public class PagedResponse<TOutput> : BaseResponse<TOutput>
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    [JsonPropertyName("firstPage")]
    public Uri FirstPage { get; set; }
    [JsonPropertyName("lastPage")]
    public Uri LastPage { get; set; }
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    [JsonPropertyName("totalRecords")]
    public int TotalRecords { get; set; }

    public PagedResponse() { }

    public PagedResponse(TOutput data, int pageNumber, int pageSize)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.Data = data;
        this.Message = null;
        this.Errors = null;
    }

    public PagedResponse(TOutput data, int pageNumber, int pageSize, int totalPages, int totalRecords)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.Data = data;
        this.TotalPages = totalPages;
        this.TotalRecords = totalRecords;
        this.Message = null;
        this.Errors = null;
    }
}
