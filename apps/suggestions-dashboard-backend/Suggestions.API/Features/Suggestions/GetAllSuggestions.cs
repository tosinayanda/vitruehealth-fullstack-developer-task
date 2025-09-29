using Suggestions.API.Contracts;
using Suggestions.API.Features.Suggestions.Dtos;
using Suggestions.API.Shared.Constants;


namespace Suggestions.API.Features.Suggestions;

public record SuggestionsSearchFilter(
    string? Status = null, // Filter by SuggestionStatus
    string? Type = null,   // Filter by SuggestionType
    string? Priority = null, // Filter by SuggestionPriority
    string? Source = null, // Filter by SuggestionSource
    Guid? EmployeeId = null // Filter by specific Employee
);

public static class GetAllSuggestions
{
    // Endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Suggestions.All, async ([AsParameters] PagingParameters paging, [AsParameters] SuggestionsSearchFilter criteria,IMediator mediator) =>
        {
            var query = new Query(paging ?? new PagingParameters(),criteria);
            var result = await mediator.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithTags("Suggestions")
        .Produces<PagedResponse<IEnumerable<SuggestionDto?>>>()
        .Produces(StatusCodes.Status404NotFound);
    }

    // The query to fetch the data
    public record Query(PagingParameters paging, SuggestionsSearchFilter? criteria) : IRequest<PagedResponse<IEnumerable<SuggestionDto?>>>;

    // The handler to execute the query
    public class Handler : IRequestHandler<Query, PagedResponse<IEnumerable<SuggestionDto?>>>
    {
        private readonly SuggestionsContext _db;

        public Handler(SuggestionsContext db) => _db = db;

        public async Task<PagedResponse<IEnumerable<SuggestionDto?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Performing a complex query joining multiple tables
            IQueryable<Suggestion> searchQuery = _db.Suggestions
                .Include(s => s.Employee)
                .Include(x => x.CreatedByAdmin);

            if (request.criteria != null)
            {
                if (!string.IsNullOrEmpty(request.criteria.Status) && Enum.TryParse<SuggestionStatus>(request.criteria.Status, true, out var status))
                {
                    searchQuery = searchQuery.Where(s => s.Status == status);
                }
                if (!string.IsNullOrEmpty(request.criteria.Type) && Enum.TryParse<SuggestionType>(request.criteria.Type, true, out var type))
                {
                    searchQuery = searchQuery.Where(s => s.Type == type);
                }
                if (!string.IsNullOrEmpty(request.criteria.Priority) && Enum.TryParse<SuggestionPriority>(request.criteria.Priority, true, out var priority))
                {
                    searchQuery = searchQuery.Where(s => s.Priority == priority);
                }
                if (!string.IsNullOrEmpty(request.criteria.Source) && Enum.TryParse<SuggestionSource>(request.criteria.Source, true, out var source))
                {
                    searchQuery = searchQuery.Where(s => s.Source == source);
                }
                if (request.criteria.EmployeeId.HasValue)
                {
                    searchQuery = searchQuery.Where(s => s.EmployeeId == request.criteria.EmployeeId.Value);
                }
            }
            
            var totalRecords = await searchQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.paging.PageSize);

            var response = await searchQuery
                .Skip((request.paging.PageNumber - 1) * request.paging.PageSize)
                .Take(request.paging.PageSize)
                .Select(s => new SuggestionDto()
                {
                    Id = s.Id,
                    Description = s.Description,
                    Status = s.Status.ToString(),
                    Priority = s.Priority.ToString(),
                    Type = s.Type.ToString(),
                    Source = s.Source.ToString(),
                    Notes = s.Notes,
                    EmployeeId = s.Employee.Id,
                    EmployeeName = s.Employee.Name,
                    CreatedBy = s.CreatedByAdmin != null ? s.CreatedByAdmin.Username : null,
                    DateCreated = s.DateCreated,
                    DateUpdated = s.DateUpdated
                })
                .AsNoTracking()
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync(cancellationToken);

            return new PagedResponse<IEnumerable<SuggestionDto?>>(response, request.paging.PageNumber, request.paging.PageSize, totalPages, totalRecords);
        }
    }
}