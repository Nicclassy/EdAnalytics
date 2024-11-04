namespace EdAnalytics.Models;

public sealed record UserAnalytics(
    User User,
    Tutorial Tutorial,
    UserThreads Threads,
    UserResponses Responses,
    FavourableReactions Reactions,
    DiscussionStatistics Discussion
);