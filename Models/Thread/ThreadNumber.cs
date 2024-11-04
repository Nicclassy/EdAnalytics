namespace EdAnalytics.Models;

public readonly record struct ThreadNumber(int Value)
{
    public static ThreadNumber Parse(int? value) =>
        new ThreadNumber(value ?? throw new ArgumentException("value cannot be null"));
}