namespace Ed.Analytics.Models;

public abstract record CreationDate 
{
    public static CreationDate Parse(string? value) =>
        !string.IsNullOrWhiteSpace(value) ? new Date(DateTime.Parse(value)) : new DateAbsent();
}

public sealed record DateAbsent : CreationDate;
public sealed record Date(DateTime DateTime) : CreationDate;