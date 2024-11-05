namespace EdAnalytics.Models;

public sealed record User(
    Account Account,
    Tutorial Tutorial,
    UserThreads Threads,
    UserResponses Responses,
    FavourableReactions Reactions,
    DiscussionStatistics Discussion
);