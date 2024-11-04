namespace EdAnalytics.Models;

public enum Role { Unknown, Student, Staff, Admin }

public sealed record User(string Name, string Email, Role Role)
{
    public static User Parse(string? name, string? email, string? role) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        var userRole = !string.IsNullOrWhiteSpace(role) ? 
            (Role) Enum.Parse(typeof(Role), role, true) :
            Role.Unknown;
        
        return new User(
            name, 
            email, 
            userRole
        );
    }
}