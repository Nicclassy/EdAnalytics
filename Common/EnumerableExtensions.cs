namespace EdAnalytics.Common;

public static class EnumerableExtensions
{
    public static void ForEachWriteLine<T>(this IEnumerable<T> enumerable)
    {
        foreach (T value in enumerable)
            Console.WriteLine(value);
    }
}