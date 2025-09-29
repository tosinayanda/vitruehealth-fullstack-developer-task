using Suggestions.API.Contracts;
using Suggestions.API.Features.Employees.Dtos;
using Suggestions.API.Features.Suggestions.Dtos;
using Suggestions.API.Shared.Constants;

namespace Suggestions.API.Features.Employees;

public record EmployeeSearchFilter(
    string? Name = null, // Filter by Employee Name
    string? Department = null, // Filter by Department
    Guid? EmployeeId = null // Filter by specific Employee
);

public static class GetAllEmployees
{
    // Endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Employees.All, async ([AsParameters] PagingParameters paging, [AsParameters] EmployeeSearchFilter criteria, IMediator mediator) =>
        {
            var query = new Query(paging ?? new PagingParameters(), criteria);
            var result = await mediator.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithTags("Employees")
        .Produces<PagedResponse<IEnumerable<EmployeeDto?>>>()
        .Produces(StatusCodes.Status404NotFound);
    }

    // The query to fetch the data
    public record Query(PagingParameters paging, EmployeeSearchFilter? criteria) : IRequest<PagedResponse<IEnumerable<EmployeeDto?>>>;

    // The handler to execute the query
    public class Handler : IRequestHandler<Query, PagedResponse<IEnumerable<EmployeeDto?>>>
    {
        private readonly SuggestionsContext _db;
        private readonly ILogger<Handler> _logger;

        public Handler(SuggestionsContext db, ILogger<Handler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResponse<IEnumerable<EmployeeDto?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetAllEmployees query");

                _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(request));

                // Performing a complex query joining multiple tables
                IQueryable<Employee> searchQuery = _db.Employees
                        .Include(s => s.Suggestions)
                            .ThenInclude(s => s.CreatedByAdmin)
                        .Include(s => s.Department);

                if (request.criteria != null)
                {
                    if (!string.IsNullOrEmpty(request.criteria.Name))
                    {
                        searchQuery = searchQuery.Where(e => e.Name.Contains(request.criteria.Name));
                    }

                    if (!string.IsNullOrEmpty(request.criteria.Department))
                    {
                        searchQuery = searchQuery.Where(e => e.Department.Name == request.criteria.Department);
                    }

                    if (request.criteria.EmployeeId.HasValue)
                    {
                        searchQuery = searchQuery.Where(e => e.Id == request.criteria.EmployeeId.Value);
                    }
                }

                var response = await searchQuery
                    .Skip((request.paging.PageNumber - 1) * request.paging.PageSize)
                    .Take(request.paging.PageSize)
                                    .Select(e => new EmployeeDto()
                                    {
                                        Id = e.Id,
                                        Name = e.Name,
                                        Department = e.Department.Name,
                                        Suggestions = e.Suggestions.Select(s => new SuggestionDto()
                                        {
                                            Id = s.Id,
                                            Description = s.Description,
                                            Status = s.Status.ToString(),
                                            Priority = s.Priority.ToString(),
                                            Type = s.Type.ToString(),
                                            Source = s.Source.ToString(),
                                            Notes = s.Notes,
                                            EmployeeId = s.Employee.Id,
                                            CreatedBy = s.CreatedByAdmin != null ? s.CreatedByAdmin.Username : null,
                                            DateCreated = s.DateCreated,
                                            DateUpdated = s.DateUpdated
                                        }).ToList()
                                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new PagedResponse<IEnumerable<EmployeeDto?>>(response, request.paging.PageNumber, request.paging.PageSize);

            }
            catch (Exception ex)
            {
                // TODO
                _logger.LogError(ex, "Error occurred while handling GetAllEmployees query");
                return null;
            }

        }
    }
}