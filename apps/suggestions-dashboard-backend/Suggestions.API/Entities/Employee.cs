namespace Suggestions.API.Entities;

public enum EmployeeRiskLevel
{
    Low,
    Medium,
    High
}

public class Employee
{
    public Employee(Guid? id, string name, int departmentId, EmployeeRiskLevel riskLevel)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        DepartmentId = departmentId;
        RiskLevel = riskLevel;
    }

    protected Employee() { } // For EF Core
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public EmployeeRiskLevel RiskLevel { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
    
    public Department Department { get; set; }
    public ICollection<Suggestion> Suggestions { get; set; } = new List<Suggestion>();
}