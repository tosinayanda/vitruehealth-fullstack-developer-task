namespace Suggestions.API.Extensions;

public static class EndpointMappingExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        Features.Suggestions.CreateOrUpdateSuggestion.MapEndpoint(app);
        Features.Suggestions.GetAllSuggestions.MapEndpoint(app);
        Features.Suggestions.GetSuggestionById.MapEndpoint(app);

        Features.Employees.GetAllEmployees.MapEndpoint(app);
        Features.Employees.GetEmployeeById.MapEndpoint(app);

        
    }
}
