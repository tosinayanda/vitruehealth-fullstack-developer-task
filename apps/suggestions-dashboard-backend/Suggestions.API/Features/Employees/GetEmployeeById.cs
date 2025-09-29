using Suggestions.API.Contracts;
using Suggestions.API.Features.Employees.Dtos;
using Suggestions.API.Features.Suggestions.Dtos;
using Suggestions.API.Shared.Constants;

namespace Suggestions.API.Features.Employees;

public static class GetEmployeeById
{
    // Endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Employees.GetById, async (Guid id, IMediator mediator) =>
        {
            var query = new Query(id);
            var result = await mediator.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithTags("Employees")
        .Produces<BaseResponse<EmployeeDto?>>()
        .Produces(StatusCodes.Status404NotFound);
    }

    // The query to fetch the data
    public record Query(Guid Id) : IRequest<BaseResponse<EmployeeDto?>>;

    // The handler to execute the query
    public class Handler : IRequestHandler<Query, BaseResponse<EmployeeDto?>>
    {
        private readonly SuggestionsContext _db;
        private readonly ILogger<Handler> _logger;

        public Handler(SuggestionsContext db, ILogger<Handler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<BaseResponse<EmployeeDto?>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Performing a complex query joining multiple tables
            var response = await _db.Employees
                .Where(s => s.Id == request.Id)
                    .Include(s => s.Suggestions)
                        .ThenInclude(s => s.CreatedByAdmin)
                    .Include(s => s.Department)
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
                .FirstOrDefaultAsync(cancellationToken);

            return new BaseResponse<EmployeeDto?>(response);
        }
    }
}