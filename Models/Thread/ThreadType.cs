namespace Ed.Analytics.Models;

public enum ThreadType { Announcement, Question, Post }

public static class ThreadTypeExtensions 
{
    public static ThreadType Parse(string? value) =>
        !string.IsNullOrWhiteSpace(value) ?
            Enum.Parse<ThreadType>(value, true) :
            throw new ArgumentException($"Unknown thread type '{value}'");
}