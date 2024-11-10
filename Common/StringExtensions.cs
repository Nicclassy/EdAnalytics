using System.Text.RegularExpressions;

namespace Ed.Analytics.Common;

public static class StringExtensions
{
    public static int CountCaseInsensitive(this string source, string value) =>
        Regex.Count(source, Regex.Escape(value), RegexOptions.IgnoreCase);

    public static bool ContainsEmoji(this string source) =>
        Enumerable.Range(0, source.Length).Any(index => 
        {
            var codePoint = char.IsSurrogatePair(source, index) 
                ? char.ConvertToUtf32(source, index++)
                : char.ConvertToUtf32(source, index);
            return (codePoint >= 0x1F600 && codePoint <= 0xF614F) || 
                    (codePoint >= 0x1F300 && codePoint <= 0x1F5FF) || 
                    (codePoint >= 0x1F680 && codePoint <= 0x1F6FF) ||
                    (codePoint >= 0x2600 && codePoint <= 0x26FF) ||
                    (codePoint >= 0x2700 && codePoint <= 0x27BF) ||
                    (codePoint >= 0xFE00 && codePoint <= 0xFE0F) ||
                    (codePoint >= 0x1F900 && codePoint <= 0x1F9FF) ||
                    (codePoint >= 0x1F1E6 && codePoint <= 0x1F1FF);
        });
}