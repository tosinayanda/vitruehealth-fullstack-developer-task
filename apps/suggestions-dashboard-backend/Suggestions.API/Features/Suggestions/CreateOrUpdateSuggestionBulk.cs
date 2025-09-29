using Suggestions.API.Shared.Constants;

namespace Suggestions.API.Features.Suggestions;

public static class CreateOrUpdateSuggestionBulk
{
    // The public-facing endpoint definition
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Suggestions.BulkCreateOrUpdate, async (Command command, IMediator mediator) =>
        {
            var suggestionId = await mediator.Send(command);
            return Results.NoContent();
        })
        .WithTags("Suggestions")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }

    // The command record that encapsulates the request data
    public record Items(
    Guid? Id,
    string? Description,
    string Source,
    Guid EmployeeId,
    string? Notes,
    int? CreatedByAdminId,
    string Type,
    string Status,
    string Priority);

    public record Command(Items[] Items) : IRequest<Unit>;

    // The validator for the command
    public class Validator : AbstractValidator<Command>
    {
        public Validator(SuggestionsContext db)
        {
            // Rule for the overall collection: ensures the array isn't null or empty
            RuleFor(x => x.Items).NotEmpty().WithMessage("The request must contain at least one item.");

            // Use RuleForEach to apply validation rules to every item in the array
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                // Validation rules for each individual item
                item.RuleFor(i => i.Description).NotEmpty().MaximumLength(500);
                item.RuleFor(i => i.Source).NotEmpty();
                item.RuleFor(i => i.EmployeeId).MustAsync(async (id, token) =>
                    await db.Employees.AnyAsync(e => e.Id == id, token))
                    .WithMessage("Employee does not exist.");
                item.RuleFor(i => i.Type).NotEmpty();
                item.RuleFor(i => i.Status).NotEmpty();
                item.RuleFor(i => i.Priority).NotEmpty();
            });
        }
    }

    // The handler that contains the business logic
    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly SuggestionsContext _db;

        public Handler(SuggestionsContext db) => _db = db;

        // The Handle method now returns Task<Unit>
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var employeeIds = request.Items
                .Select(i => i.EmployeeId)
                .Distinct()
                .ToArray();

            var employees = await _db.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e, cancellationToken);

            var itemsToUpdate = request.Items.Where(i => i.Id.HasValue).ToList();
            var itemsToCreate = request.Items.Where(i => !i.Id.HasValue).ToList();

            // --- UPDATE LOGIC ---
            
            var existingIds = itemsToUpdate.Select(i => i.Id!.Value).ToArray();
            var suggestionsToUpdate = await _db.Suggestions
                .Where(s => existingIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s, cancellationToken);

            foreach (var item in itemsToUpdate)
            {
                if (suggestionsToUpdate.TryGetValue(item.Id!.Value, out var suggestion))
                {
                    // Map properties from the request item to the existing suggestion entity
                    suggestion.Description = item.Description ?? string.Empty;
                    suggestion.Source = Enum.Parse<SuggestionSource>(item.Source, true);
                    suggestion.Type = Enum.Parse<SuggestionType>(item.Type, true);
                    suggestion.Status = Enum.Parse<SuggestionStatus>(item.Status, true);
                    suggestion.Priority = Enum.Parse<SuggestionPriority>(item.Priority, true);
                    suggestion.Notes = item.Notes;
                    suggestion.DateUpdated = DateTimeOffset.UtcNow;
                    
                    _db.Suggestions.Update(suggestion);
                }
                else
                {
                    // Handle case where an ID was provided but the suggestion wasn't found
                    // Throwing here ensures atomicity if any required update fails.
                    throw new KeyNotFoundException($"Suggestion with ID {item.Id} not found for update.");
                }
            }

            // --- ADD LOGIC ---
            
            var newSuggestions = new List<Suggestion>();
            foreach (var item in itemsToCreate)
            {
                if (employees.TryGetValue(item.EmployeeId, out var employee))
                {
                    var newSuggestion = new Suggestion(
                        Guid.NewGuid(), // Assuming a constructor that takes ID or generates it
                        item.Description ?? string.Empty,
                        Enum.Parse<SuggestionSource>(item.Source, true),
                        Enum.Parse<SuggestionType>(item.Type, true),
                        Enum.Parse<SuggestionStatus>(item.Status, true),
                        Enum.Parse<SuggestionPriority>(item.Priority, true),
                        item.EmployeeId,
                        employee.DepartmentId,
                        item.Notes,
                        item.CreatedByAdminId,
                        DateTimeOffset.UtcNow,
                        DateTimeOffset.UtcNow
                    );
                    newSuggestions.Add(newSuggestion);
                }
                else
                {
                    // This should ideally be caught by the Validator, but it's a good safety check
                    throw new KeyNotFoundException($"Employee with ID {item.EmployeeId} not found for creation.");
                }
            }

            // 5. Add all new entities
            await _db.Suggestions.AddRangeAsync(newSuggestions, cancellationToken);

            // 6. Save all changes (updates and creations) in a single transaction
            await _db.SaveChangesAsync(cancellationToken);

            // Return Unit.Value to signal completion
            return Unit.Value;
        }
    }
}