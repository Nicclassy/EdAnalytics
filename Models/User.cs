namespace Ed.Analytics.Models;

public sealed record User(
    Account Account,
    Tutorial Tutorial,
    UserThreads Threads,
    UserResponses Responses,
    FavourableReactions Reactions,
    UserDiscussionStatistics Discussion
);