namespace Ed.Analytics.Models;

public sealed record ThreadMetadata(ContentURL Url, CreationTime CreationTime, ThreadType Type, ThreadNumber Number)
{
    public static ThreadMetadata Parse(string? url, string? creationTime, string? threadType, int? threadNumber) =>
        new(
            ContentURL.Parse(url),
            CreationTime.Parse(creationTime),
            ThreadTypeExtensions.Parse(threadType),
            ThreadNumber.Parse(threadNumber)
        );
}