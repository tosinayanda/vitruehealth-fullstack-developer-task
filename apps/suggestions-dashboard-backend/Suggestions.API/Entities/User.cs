namespace Suggestions.API.Entities;

public class User
{
    public User(string emailAddress, string displayName, string firstName, string lastName, string username, string? password, string role="User")
    {
        EmailAddress = emailAddress;
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        Password = password;
        Role = role;
    }

    protected User() { } // For EF Core
    
    public int Id { get; set; }
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string? Password { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
}