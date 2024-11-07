namespace EdAnalytics.Models;

public enum DiscussionRole { Unknown, Student, Observer, Staff, Admin }

public sealed record Account(string Name, string Email, DiscussionRole Role)
{
    public static Account Parse(string? name, string? email, string? role) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        var userRole = !string.IsNullOrWhiteSpace(role) ? 
            (DiscussionRole) Enum.Parse(typeof(DiscussionRole), role, true) :
            DiscussionRole.Unknown;
        
        return new(
            name, 
            email, 
            userRole
        );
    }
}