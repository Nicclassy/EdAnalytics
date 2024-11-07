namespace Ed.Analytics.Models;

public readonly record struct ContentURL(Uri Uri)
{
    public static ContentURL Parse(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out Uri? result) && result is Uri uri ? 
        new ContentURL(uri) : throw new ArgumentException($"URL {url} is in an invalid format.");
}