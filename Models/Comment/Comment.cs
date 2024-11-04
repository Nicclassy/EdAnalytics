using System.Collections.Immutable;

namespace EdAnalytics.Models;

public sealed record Comment(
    User Poster,
    ContentText Text,
    CommentMetadata Metadata,
    CommentAttributes Attributes,
    ImmutableArray<Comment> Comments
);