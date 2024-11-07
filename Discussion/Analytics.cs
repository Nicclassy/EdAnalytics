using System.Collections;
using System.Collections.Immutable;

using EdAnalytics.Models;

namespace EdAnalytics.EdDiscussion;

public sealed class Analytics(ImmutableArray<User> _analytics) : IEnumerable<User>
{
    public Analytics(IEnumerable<User> analytics): this(analytics.ToImmutableArray()) {}

    public User this[Index index] => _analytics[index];

    public IEnumerator<User> GetEnumerator() =>
        _analytics.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class AnalyticsFiltering
{
    public static Analytics OnDay(this Analytics analytics, DayAbbreviated day) =>
        new(analytics
        .Where(user => user.Tutorial.Day == day));

    public static Analytics WithTutor(this Analytics analytics, string tutor) =>
        new(analytics
            .Where(user => user.Tutorial.Tutor == tutor));

    public static Analytics StartingAtOrAfter(this Analytics analytics, int startHour) =>
        new(analytics
            .Where(user => user.Tutorial.StartHour >= startHour));
}

public static class AnalyticsStatistics
{
    public const int DefaultTopN = 5;

    public static Analytics TopEndorsed(this Analytics analytics, int count = DefaultTopN) =>
        new(analytics
            .OrderBy(user => user.Reactions.Endorsements)
            .Take(count));

    public static IEnumerable<(string, int)> GroupByTutor(this Analytics analytics, Func<User, int> predicate)
    {
        var queryTutorGroups =
            from user in analytics
            where user.Account.Role == Role.Student
            group user by user.Tutorial.Tutor;

        var queryResult =
            from g in queryTutorGroups
            let value = g.Sum(predicate)
            orderby value descending
            select (g.Key, value);

        return queryResult;
    }

    public static IEnumerable<(string, int)> TutorsByHearts(this Analytics analytics) =>
        GroupByTutor(analytics, student => student.Reactions.Hearts);
}