using System.Collections;
using System.Collections.Immutable;

using Ed.Analytics.Models;

namespace Ed.Analytics.EdDiscussion;

public sealed class UserAnalytics(ImmutableArray<User> _analytics) : IEnumerable<User>
{
    public UserAnalytics(IEnumerable<User> analytics): this(analytics.ToImmutableArray()) {}

    public User this[Index index] => _analytics[index];

    public IEnumerator<User> GetEnumerator() =>
        _analytics.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class AnalyticsFiltering
{
    public static UserAnalytics OnDay(this UserAnalytics analytics, DayAbbreviated day) =>
        new(analytics
        .Where(user => user.Tutorial.Day == day));

    public static UserAnalytics WithTutor(this UserAnalytics analytics, string tutor) =>
        new(analytics
            .Where(user => user.Tutorial.Tutor == tutor));

    public static UserAnalytics StartingAtOrAfter(this UserAnalytics analytics, int startHour) =>
        new(analytics
            .Where(user => user.Tutorial.StartHour >= startHour));
}

public static class AnalyticsStatistics
{
    public const int DefaultTopN = 5;

    public static UserAnalytics TopEndorsed(this UserAnalytics analytics, int count = DefaultTopN) =>
        new(analytics
            .OrderBy(user => user.Reactions.Endorsements)
            .Take(count));

    public static IEnumerable<(string, int)> GroupByTutor(this UserAnalytics analytics, Func<User, int> predicate)
    {
        var queryTutorGroups =
            from user in analytics
            where user.Account.Role == DiscussionRole.Student
            group user by user.Tutorial.Tutor;

        var queryResult =
            from g in queryTutorGroups
            let value = g.Sum(predicate)
            orderby value descending
            select (g.Key, value);

        return queryResult;
    }

    public static IEnumerable<(string, int)> TutorsByHearts(this UserAnalytics analytics) =>
        GroupByTutor(analytics, student => student.Reactions.Hearts);
}