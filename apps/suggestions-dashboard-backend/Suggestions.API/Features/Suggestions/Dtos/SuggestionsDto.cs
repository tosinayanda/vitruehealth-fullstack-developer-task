using System.Text.Json.Serialization;

namespace Suggestions.API.Features.Suggestions.Dtos;

public record class SuggestionDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("priority")]
    public string Priority { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    // Optional field
    [JsonPropertyName("createdBy")]
    public string? CreatedBy { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTimeOffset DateCreated { get; set; }

    [JsonPropertyName("dateUpdated")]
    public DateTimeOffset? DateUpdated { get; set; }

    // Optional field
    [JsonPropertyName("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
