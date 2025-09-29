namespace Suggestions.API.Contracts;

public record PagingParameters(
    int PageNumber = 1,
    int PageSize = 10
    // string SortBy = "DateCreated",
    // bool SortAscending = false
);