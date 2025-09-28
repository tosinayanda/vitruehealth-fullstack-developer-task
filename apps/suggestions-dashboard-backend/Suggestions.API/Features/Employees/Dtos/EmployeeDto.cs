using System.Text.Json.Serialization;

namespace Suggestions.API.Features.Employees.Dtos;

public record class EmployeeDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("department")]
    public string Department { get; set; }

    [JsonPropertyName("riskLevel")]
    public string RiskLevel { get; set; }
}
