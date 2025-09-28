namespace Suggestions.API.Entities;

public class AuditLog
{
    protected AuditLog() { } // For EF Core
    
    public int Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public required string ActionType { get; set; }
    public required string EntityName { get; set; }
    public int RecordId { get; set; }
    public required string Changes { get; set; }
    public int? AdminId { get; set; }

    public static AuditLog CreateFromEntry(EntityEntry entry)
    {
        var changes = new Dictionary<string, object?>();
        foreach (var prop in entry.Properties)
        {
            if (prop.IsModified)
            {
                changes[prop.Metadata.Name] = new
                {
                    Old = prop.OriginalValue,
                    New = prop.CurrentValue
                };
            }
        }

        return new AuditLog
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = entry.State.ToString(), // Added, Modified, Deleted
            EntityName = entry.Entity.GetType().Name,
            RecordId = (int)entry.Property("Id").CurrentValue!,
            Changes = JsonSerializer.Serialize(changes),
            AdminId = 1 // Hardcoded for demo; in a real app, get this from HttpContext
        };
    }
}