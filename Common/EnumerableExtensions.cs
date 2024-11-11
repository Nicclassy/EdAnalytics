namespace Ed.Analytics.Common;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Inspect<T>(this IEnumerable<T> source, Action<T> action) =>
        source.Select(value => 
        {
            action(value); 
            return value;
        });

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

    public static void ForEachCountedWriteDelimited<T, U>(this IEnumerable<(T, U)> source, string delimiter = " ")
    {
        void WriteCounted(T value, U count) => Console.Write($"{value} ({count})");

        if (!source.Any())
        {
            Console.WriteLine();
            return;
        }

        var first = source.First();
        WriteCounted(first.Item1, first.Item2);
        foreach ((T value, U count) in source.Skip(1))
        {
            Console.Write(delimiter);
            WriteCounted(value, count);
        }
        Console.WriteLine();
    }

    public static void ForEachCountedWriteCommaSeparated<T, U>(this IEnumerable<(T, U)> elements) =>
        ForEachCountedWriteDelimited(elements, ", ");

    public static void ForEachWriteCommaSeparated<T>(this IEnumerable<T> elements) =>
        ForEachWriteDelimited(elements, ", ");

    public static void WriteEnumeratedCount<T, U>(this IEnumerable<(T, U)> source) 
    {
        foreach ((int i, (T t, U u)) in source.Enumerate(1))
            Console.WriteLine($"{i}. {t} ({u})");
    }
}