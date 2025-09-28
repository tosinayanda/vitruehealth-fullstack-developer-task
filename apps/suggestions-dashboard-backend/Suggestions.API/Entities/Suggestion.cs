namespace Suggestions.API.Entities;

public enum SuggestionStatus
{
    Pending = 1,
    InProgress = 2,
    Overdue = 3,
    Completed = 3,
    Dismissed = 4
}

public enum SuggestionPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum SuggestionType
{
    Equipment = 1,
    Exercise = 2,
    Behavioural = 3,
    Lifestyle = 4
}

public enum SuggestionSource
{
    Vida = 1,
    Admin = 2,
}

public class Suggestion
{
    public Suggestion(Guid? id, string description, SuggestionSource source, SuggestionType type, SuggestionStatus status, SuggestionPriority priority,
    Guid employeeId, int departmentId, string? notes,  int? createdByAdminId, DateTimeOffset? dateCreated=null, DateTimeOffset? dateUpdated=null)
    {
        Id = id ?? Guid.NewGuid();
        Description = description;
        Source = source;
        Type = type;
        Status = status;
        Priority = priority;
        Notes = notes;
        EmployeeId = employeeId;
        DepartmentId = departmentId;
        CreatedByAdminId = createdByAdminId;
        DateCreated = dateCreated ?? DateTimeOffset.UtcNow;
        DateUpdated = dateUpdated;
    }

    protected Suggestion() { } // For EF Core

    public Guid Id { get; set; }
    public string Description { get; set; }
    public SuggestionSource Source { get; set; }
    public SuggestionType Type { get; set; }
    public SuggestionStatus Status { get; set; }
    public SuggestionPriority Priority { get; set; }
    public string? Notes { get; set; }
    public Guid EmployeeId { get; set; }
    public int DepartmentId { get; set; }
    public int? CreatedByAdminId { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
    public DateTimeOffset? DateCompleted { get; set; }

    public Department Department { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public User? CreatedByAdmin { get; set; }
}