namespace Suggestions.API.Database;

using System.Text.Json.Serialization;

using Suggestions.API.Features.Suggestions.Dtos;
using Suggestions.API.Features.Employees.Dtos;

public class SeedData
{
    [JsonPropertyName("employees")]
    public List<EmployeeDto> Employees { get; set; }

    [JsonPropertyName("suggestions")]
    public List<SuggestionDto> Suggestions { get; set; }
}

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(IApplicationBuilder app)
    {
        // We need to create a new scope to resolve scoped services like DbContext
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SuggestionsContext>();

        // Ensure the database is created
        await dbContext.Database.EnsureCreatedAsync();

        var seedData = await ReadSeedDataFromJsonFileAsync();
        if (seedData == null)
        {
            Console.WriteLine("No seed data available to seed the database.");
            return;
        }

        var uniqueDepartmentNames = seedData.Employees
            .Select(e => e.Department)
            .Distinct()
            .ToList();

        var uniqueAdminUserNames = seedData.Suggestions
            .Where(s => !string.IsNullOrEmpty(s.CreatedBy))
            .Select(s => s.CreatedBy!)
            .Distinct()
            .ToList();

        // Check if data already exists to avoid duplicates
        if (!dbContext.Departments.Any())
        {
            foreach (var dept in uniqueDepartmentNames)
            {
                await dbContext.Departments.AddAsync(new Department(dept));
            }

            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Admins.Any())
        {
            foreach (var admin in uniqueAdminUserNames)
            {
                dbContext.Admins.Add(new User(admin, admin, admin, admin, admin, null, "Admin"));
            }

            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Employees.Any())
        {
            foreach (var employee in seedData.Employees)
            {
                dbContext.Employees.Add(new Employee(
                    employee.Id,
                    employee.Name,
                    dbContext.Departments.First(d => d.Name == employee.Department).Id,
                    Enum.Parse<EmployeeRiskLevel>(employee.RiskLevel, true)
                ));
            }

            await dbContext.SaveChangesAsync();
        }
        
        if(!dbContext.Suggestions.Any())
        {
            foreach (var suggestion in seedData.Suggestions)
            {
                var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == suggestion.EmployeeId);
                if (employee == null)
                {
                    Console.WriteLine($"Employee with ID {suggestion.EmployeeId} not found. Skipping suggestion.");
                    continue;
                }

                int? createdByAdminId = null;
                if (!string.IsNullOrEmpty(suggestion.CreatedBy))
                {
                    var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Username == suggestion.CreatedBy);
                    if (admin != null)
                    {
                        createdByAdminId = admin.Id;
                    }
                }

                var newSuggestion = new Suggestion(
                    suggestion.Id,
                    suggestion.Description,
                    Enum.Parse<SuggestionSource>(suggestion.Source, true),
                    Enum.Parse<SuggestionType>(suggestion.Type, true),
                    Enum.Parse<SuggestionStatus>(suggestion.Status, true),
                    Enum.Parse<SuggestionPriority>(suggestion.Priority, true),
                    employee.Id,
                    employee.DepartmentId,
                    suggestion.Notes,
                    createdByAdminId,
                    suggestion.DateCreated,
                    suggestion.DateUpdated
                );

                dbContext.Suggestions.Add(newSuggestion);
            }

            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task<SeedData?> ReadSeedDataFromJsonFileAsync()
    {
        var filePath = "sample-data.json";

        if (!File.Exists(filePath))
        {
            // Log an error or throw an exception if the file isn't found
            Console.WriteLine($"Seed file not found at: {filePath}");
            return null;
        }

        string jsonContent = await File.ReadAllTextAsync(filePath);

        var seedData = JsonSerializer.Deserialize<SeedData>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        });

        if (seedData == null)
        {
            Console.WriteLine("Failed to deserialize seed data.");
            return null;
        }

        return seedData;
    }
}