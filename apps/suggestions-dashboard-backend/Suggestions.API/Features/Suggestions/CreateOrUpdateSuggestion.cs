using Suggestions.API.Shared.Constants;

namespace Suggestions.API.Features.Suggestions;

public static class CreateOrUpdateSuggestion
{
    // The public-facing endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {

        app.MapPost(Routes.Suggestions.Create, async (Command command, IMediator mediator) =>
        {
            var suggestionId = await mediator.Send(command);
            return Results.Created($"/suggestions/{suggestionId}", new { id = suggestionId });
        })
        .WithTags("Suggestions")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPost(Routes.Suggestions.Update, async (Command command, IMediator mediator) =>
        {
            var suggestionId = await mediator.Send(command);
            return Results.Ok(new { id = suggestionId });
        })
        .WithTags("Suggestions")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }

    // The command record that encapsulates the request data
    public record Command(
        Guid? Id,
        string Description,
        string Source,
        Guid EmployeeId,
        string Notes,
        int CreatedByAdminId,
        int TypeId,
        int StatusId,
        int PriorityId) : IRequest<Guid>;

    // The validator for the command
    public class Validator : AbstractValidator<Command>
    {
        public Validator(SuggestionsContext db)
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.EmployeeId).MustAsync(async (id, token) =>
                await db.Employees.AnyAsync(e => e.Id == id, token))
                .WithMessage("Employee does not exist.");
            // Add similar rules for TypeId, StatusId, PriorityId
        }
    }

    // The handler that contains the business logic
    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly SuggestionsContext _db;

        public Handler(SuggestionsContext db) => _db = db;

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            Suggestion? suggestion;

            var employee = await _db.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);

            if (request.Id.HasValue)
            {
                // Update existing suggestion
                suggestion = await _db.Suggestions.FindAsync(new object[] { request.Id.Value }, cancellationToken);
                if (suggestion == null)
                {
                    throw new KeyNotFoundException("Suggestion not found.");
                }

                // suggestion.Description = request.Description;
                // suggestion.Source = Enum.Parse<SuggestionSource>(request.Source, true);
                // suggestion.Type = Enum.Parse<SuggestionType>(request.TypeId.ToString(), true);
                // suggestion.Priority = Enum.Parse<SuggestionPriority>(request.PriorityId.ToString(), true);
                // suggestion.EmployeeId = request.EmployeeId;
                // suggestion.Notes = request.Notes;
                
                suggestion.Status = Enum.Parse<SuggestionStatus>(request.StatusId.ToString(), true);
                suggestion.DateUpdated = DateTimeOffset.UtcNow;

                _db.Suggestions.Update(suggestion);

                // The AuditingInterceptor will automatically create the audit log here
                await _db.SaveChangesAsync(cancellationToken);
            }
            else
            {
                // Create new suggestion
                suggestion = new Suggestion(
                    null,
                    request.Description,
                    Enum.Parse<SuggestionSource>(request.Source, true),
                    Enum.Parse<SuggestionType>(request.TypeId.ToString(), true),
                    Enum.Parse<SuggestionStatus>(request.StatusId.ToString(), true),
                    Enum.Parse<SuggestionPriority>(request.PriorityId.ToString(), true),
                    request.EmployeeId,
                    employee!.DepartmentId,
                    request.Notes,
                    request.CreatedByAdminId,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow
                );

                await _db.Suggestions.AddAsync(suggestion);

                // The AuditingInterceptor will automatically create the audit log here
                await _db.SaveChangesAsync(cancellationToken);
            }

            return suggestion.Id;
        }
    }
}