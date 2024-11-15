using System.Collections;
using System.Collections.Immutable;
using Ed.Analytics.Common;
using Ed.Analytics.Models;

namespace Ed.Analytics.Discussion;

public delegate IComparable UserSelector(User user);
public delegate string UserFormatter(User user);

public sealed class UserAnalytics(ImmutableArray<User> _analytics) : IEnumerable<User>
{
    public const int DefaultCount = 5;

    public UserAnalytics(IEnumerable<User> analytics): this(analytics.ToImmutableArray()) {}

    public int Length => _analytics.Length;

    public User this[Index index] => _analytics[index];

    public User this[string name] => 
        _analytics.FirstOrDefault(user => name.Equals(user.Account.Name)) 
        ?? throw new KeyNotFoundException("user not found");

    public IEnumerator<User> GetEnumerator() =>
        _analytics.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class AnalyticsOrdering
{
    public static IEnumerable<(User, IComparable)> CountAndOrderBy(
        this UserAnalytics analytics, 
        UserSelector selector,
        int count = UserAnalytics.DefaultCount
    ) => analytics
            .Select(user => (user, selector(user)))
            .OrderByDescending(pair => pair.Item2)
            .Take(count);

    public static IEnumerable<(string, IComparable)> CountFormatAndOrderBy(
        this UserAnalytics analytics, 
        UserSelector selector,
        UserFormatter formatter,
        int count = UserAnalytics.DefaultCount
    ) => analytics
            .Select(user => (formatter(user), selector(user)))
            .OrderByDescending(pair => pair.Item2)
            .Take(count);

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

    public static UserAnalytics TopDescending(
        this UserAnalytics analytics, 
        Func<User, IComparable> predicate, 
        int count = UserAnalytics.DefaultCount
    ) =>
        new(analytics
            .OrderByDescending(predicate).Take(count));
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

    public static UserAnalytics StudentsOnly(this UserAnalytics analytics) =>
        new(analytics
            .Where(user => user.Account.Role == DiscussionRole.Student));
}

public static class AnalyticsStatistics
{
    public static UserAnalytics TopEndorsed(this UserAnalytics analytics, int count = UserAnalytics.DefaultCount) =>
        analytics.TopDescending(user => user.Reactions.Endorsements);

    public static IEnumerable<(string, int)> TutorsByHearts(this UserAnalytics analytics) =>
        analytics.GroupByTutor(student => student.Reactions.Hearts);
}

public static class UserStatistics
{
    public static double AcceptedAnswerPercentage(this User user) =>
        ((double) user.Responses.AcceptedAnswers).PercentageOf((double) user.Responses.Answers);

    public static UserAnalytics MostAcceptedAnswers(this UserAnalytics analytics, int count = UserAnalytics.DefaultCount) => 
        analytics.TopDescending(user => user.AcceptedAnswerPercentage(), count);
}   