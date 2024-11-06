using System.Collections.Immutable;

namespace EdAnalytics.Models;

public sealed record Comment(
    Account Poster,
    ContentText Text,
    CommentMetadata Metadata,
    CommentAttributes Attributes,
    ImmutableArray<Comment> Comments
);