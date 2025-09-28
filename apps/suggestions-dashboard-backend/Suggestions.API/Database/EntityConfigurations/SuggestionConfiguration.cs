using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Suggestions.API.Database.EntityConfigurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Primary Key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255); // Set a reasonable max length for the name

        builder.Property(d => d.DateCreated)
            .IsRequired();

        // Relationships
        builder.HasMany(d => d.Employees)
            .WithOne() // Employee doesn't seem to have a Department navigation property back, so we use WithOne()
            .HasForeignKey(x => x.DepartmentId)
            // EF Core will look for a shadow foreign key named 'DepartmentId' in Employee or you need to add it to the Employee entity
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd(); // Guid should be generated on add

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Note: The Department property in Employee seems to be a string name, not a foreign key to the Department entity.
        // I will configure it as a required string property. If it was intended to be a navigation property/FK, the model would need adjustment.
        builder.Property(e => e.DepartmentId)
            .IsRequired();
            // .HasMaxLength(100);

        builder.Property(e => e.RiskLevel)
            .IsRequired(); 

        builder.Property(e => e.DateCreated)
            .IsRequired();

        // Relationships
        builder.HasMany(e => e.Suggestions)
            .WithOne(s => s.Employee) // Suggestion has an Employee navigation property
            .HasForeignKey(s => s.EmployeeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // Deleting an Employee deletes their Suggestions
    }
}

public class SuggestionConfiguration : IEntityTypeConfiguration<Suggestion>
{
    public void Configure(EntityTypeBuilder<Suggestion> builder)
    {
        // Primary Key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(1000); // Set a reasonable max length for the description

        builder.Property(s => s.Source)
            .IsRequired();

        builder.Property(s => s.Type)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.Priority)
            .IsRequired();
        
        builder.Property(s => s.Notes)
            .HasMaxLength(2000); // Max length for optional notes

        builder.Property(s => s.DateCreated)
            .IsRequired();

        // Foreign Keys
        builder.Property(s => s.EmployeeId)
            .IsRequired();

        // builder.Property(s => s.DepartmentId)
        //     .IsRequired();

        // Relationships
        
        // Many-to-One with Employee
        builder.HasOne(s => s.Employee)
            .WithMany(e => e.Suggestions)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict); // Keep Suggestion if Employee is deleted if possible (or use ClientSetNull)

        // // Many-to-One with Department
        // builder.HasOne(s => s.Department)
        //     .WithMany() // Department doesn't have a navigation property for Suggestions
        //     .HasForeignKey(s => s.DepartmentId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Restrict); // Keep Suggestion if Department is deleted if possible (or use ClientSetNull)

        // Many-to-One with Admin User (optional)
        builder.HasOne(s => s.CreatedByAdmin)
            .WithMany() // User doesn't have a navigation property for Suggestions
            .HasForeignKey(s => s.CreatedByAdminId)
            .IsRequired(false) // CreatedByAdminId is nullable
            .OnDelete(DeleteBehavior.Restrict); 
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        // Unique Constraint for Username
        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.Password)
            .HasMaxLength(256); // Password should be hashed, 256 is a common size for SHA-256

        builder.Property(u => u.EmailAddress)
            .IsRequired()
            .HasMaxLength(255);

        // Unique Constraint for EmailAddress
        builder.HasIndex(u => u.EmailAddress)
            .IsUnique();

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.DateCreated)
            .IsRequired();
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Primary Key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.Timestamp)
            .IsRequired();

        builder.Property(a => a.ActionType)
            .IsRequired()
            .HasMaxLength(50); // e.g., "Added", "Modified", "Deleted"

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(100); // e.g., "Department", "Employee", "Suggestion"

        builder.Property(a => a.RecordId)
            .IsRequired(); // The Id of the record that was changed

        builder.Property(a => a.Changes)
            .IsRequired()
            .HasColumnType("nvarchar(max)"); // Changes can be a large JSON string

        // Foreign Key
        builder.Property(a => a.AdminId)
            .IsRequired(false); // AdminId is nullable (?)

        // Relationships
        // Many-to-One with Admin User (User entity)
        // This assumes the AdminId links back to the User entity (via User.Id)
        builder.HasOne<User>()
            .WithMany() // User doesn't have a navigation property for AuditLogs
            .HasForeignKey(a => a.AdminId)
            .IsRequired(false) // AdminId is nullable
            .OnDelete(DeleteBehavior.SetNull); // If the Admin is deleted, set AdminId to null
    }
}