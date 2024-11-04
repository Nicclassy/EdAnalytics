namespace EdAnalytics.Models;

public sealed record ContentURL(Uri Uri)
{
    public static ContentURL Parse(string? uriString) =>
        Uri.TryCreate(uriString, UriKind.Absolute, out Uri? result) && result is Uri uri ? 
        new ContentURL(uri) : throw new ArgumentException($"URL {uriString} is in an invalid format.");
}