namespace Suggestions.API.Entities;

public class Department
{
    public Department(string name)
    {
        Name = name;
        DateCreated = DateTimeOffset.UtcNow;
    }

    protected Department() { } // For EF Core
    
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}