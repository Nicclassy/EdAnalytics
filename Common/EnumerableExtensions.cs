namespace Ed.Analytics.Common;

public static class EnumerableExtensions
{
    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> source, int start = 0)
    {
        int i = start;
        foreach (T value in source)
            yield return (i++, value);
    }

    public static void ForEachWriteLine<T>(this IEnumerable<T> source)
    {
        foreach (T value in source)
            Console.WriteLine(value);
    }

    public static void ForEachWriteDelimited<T>(this IEnumerable<T> source, string delimiter = " ")
    {
        if (!source.Any())
        {
            Console.WriteLine();
            return;
        }

        Console.Write(source.First());
        foreach (T value in source.Skip(1))
        {
            Console.Write(delimiter);
            Console.Write(value);
        }
        Console.WriteLine();
    }

    public static void ForEachWriteCommaSeparated<T>(this IEnumerable<T> source) =>
        ForEachWriteDelimited(", ");
}