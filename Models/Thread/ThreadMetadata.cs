namespace Ed.Analytics.Models;

public sealed record ThreadMetadata(ContentURL Url, CreationDate Date, ThreadType Type, ThreadNumber Number)
{
    public static ThreadMetadata Parse(string? url, string? creationDate, string? threadType, int? threadNumber) =>
        new(
            ContentURL.Parse(url),
            CreationDate.Parse(creationDate),
            ThreadTypeExtensions.Parse(threadType),
            ThreadNumber.Parse(threadNumber)
        );
}