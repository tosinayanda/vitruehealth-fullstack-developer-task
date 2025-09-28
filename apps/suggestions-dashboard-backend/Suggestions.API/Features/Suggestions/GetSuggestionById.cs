using Suggestions.API.Contracts;
using Suggestions.API.Features.Suggestions.Dtos;
using Suggestions.API.Shared.Constants;

namespace Suggestions.API.Features.Suggestions;

public static class GetSuggestionById
{
    // Endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Suggestions.GetById, async (Guid id, IMediator mediator) =>
        {
            var query = new Query(id);
            var result = await mediator.Send(query);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithTags("Suggestions")
        .Produces<BaseResponse<SuggestionDto?>>()
        .Produces(StatusCodes.Status404NotFound);
    }

    // The query to fetch the data
    public record Query(Guid Id) : IRequest<BaseResponse<SuggestionDto?>>;

    // The handler to execute the query
    public class Handler : IRequestHandler<Query, BaseResponse<SuggestionDto?>>
    {
        private readonly SuggestionsContext _db;

        public Handler(SuggestionsContext db) => _db = db;

        public async Task<BaseResponse<SuggestionDto?>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Performing a complex query joining multiple tables
            var response = await _db.Suggestions
                .Where(s => s.Id == request.Id)
                    .Include(s => s.Employee)
                    .Include(x => x.CreatedByAdmin)
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
                    CreatedBy = s.CreatedByAdmin != null ? s.CreatedByAdmin.Username : null,
                    DateCreated = s.DateCreated,
                    DateUpdated = s.DateUpdated
                })
                .FirstOrDefaultAsync(cancellationToken);

            return new BaseResponse<SuggestionDto?>(response);
        }
    }
}