using System.Text.RegularExpressions;

namespace Ed.Analytics.Common;

public static class StringExtensions
{
    public static int CountCaseInsensitive(this string source, string value) =>
        Regex.Count(source, Regex.Escape(value), RegexOptions.IgnoreCase);
}