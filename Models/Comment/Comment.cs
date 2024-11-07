using System.Collections.Immutable;

namespace Ed.Analytics.Models;

public sealed record Comment(
    Account Poster,
    ContentText Text,
    CommentMetadata Metadata,
    CommentAttributes Attributes,
    ImmutableArray<Comment> Comments
);