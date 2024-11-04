using System.Collections.Immutable;

namespace EdAnalytics.Models;

public sealed record DiscussionThread(
    User Poster,
    ContentText Title,
    ContentText Text,
    ThreadMetadata Metadata,
    ThreadCategories Categories, 
    ThreadInteractions Interactions, 
    ThreadAttributes Attributes,
    ImmutableArray<Comment> Answers,
    ImmutableArray<Comment> Comments
);