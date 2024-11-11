using System.Collections.Immutable;
using Ed.Analytics.Common;

namespace Ed.Analytics.Models;

public sealed record DiscussionThread(
    Account Poster,
    ContentText Title,
    ContentText Content,
    ThreadMetadata Metadata,
    ThreadCategories Categories, 
    ThreadInteractions Interactions, 
    ThreadAttributes Attributes,
    ImmutableArray<Comment> Answers,
    ImmutableArray<Comment> Comments
)
{
    public string PosterProperName() => Attributes.Anonymous ? "Anonymous" : Poster.Name;

    public string InternalHyperlink() => '#' + Metadata.Number.Value.ToString();

    public string Summary() => $"'{Title.Text}' by {PosterProperName()} ({InternalHyperlink()})";

    public IEnumerable<Comment> Responses() =>
        Answers
            .Concat(Comments)
            .SelectMany(comment => comment.Subcomments())
            .Concat(Answers)
            .Concat(Comments);

    public int CountOccurrences(string value) =>
        Title.Text.CountCaseInsensitive(value)
        + Content.Text.CountCaseInsensitive(value)
        + Responses().Sum(answer => answer.Content.Text.CountCaseInsensitive(value));

    public bool IsRespondedTo() => Answers.Length > 0;
}