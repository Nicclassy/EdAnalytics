using System.Collections.Immutable;

namespace Ed.Analytics.Models;

public sealed record DiscussionThread(
    Account Poster,
    ContentText Title,
    ContentText Text,
    ThreadMetadata Metadata,
    ThreadCategories Categories, 
    ThreadInteractions Interactions, 
    ThreadAttributes Attributes,
    ImmutableArray<Comment> Answers,
    ImmutableArray<Comment> Comments
);