using Suggestions.API.Database.EntityConfigurations;

namespace Suggestions.API.Database;

public class SuggestionsContext : DbContext
{
    public SuggestionsContext(DbContextOptions<SuggestionsContext> options) : base(options) { }

    public DbSet<Suggestion> Suggestions { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Admins { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply individual configurations 
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new SuggestionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration()); 
    }
}