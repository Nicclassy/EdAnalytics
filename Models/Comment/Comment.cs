using System.Collections.Immutable;

namespace Ed.Analytics.Models;

public sealed record Comment(
    Account Poster,
    ContentText Content,
    CommentMetadata Metadata,
    CommentAttributes Attributes,
    ImmutableArray<Comment> Comments
)
{
    public IEnumerable<Comment> Subcomments()
    {
        foreach (var comment in Comments)
        {
            yield return comment;
            foreach (var subcomment in comment.Subcomments())
                yield return subcomment;
        }
    }
}