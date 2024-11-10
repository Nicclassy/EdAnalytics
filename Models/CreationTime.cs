namespace Ed.Analytics.Models;

public abstract record CreationTime 
{
    public static CreationTime Parse(string? value) =>
        !string.IsNullOrWhiteSpace(value) ? new FullTime(DateTime.Parse(value)) : new TimeAbsent();

    public abstract bool Exists();
}

public sealed record FullTime(DateTime DateTime) : CreationTime
{
    public override bool Exists() => true;
}

public sealed record TimeAbsent : CreationTime
{
    public override bool Exists() => false;
}