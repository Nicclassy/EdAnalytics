using Ed.Analytics.Models;
using Ed.Analytics.Common;

namespace Ed.Analytics.Discussion;

public sealed partial class DiscussionStatistics(UserAnalytics _analytics, Threads _threads)
{
    public UserAnalytics Analytics { get; } = _analytics;
    public Threads Threads { get; } = _threads;

    public const int DefaultCount = UserAnalytics.DefaultCount;

    public void TutorsByHearts()
    {
        var queryTutorGroups =
            from user in Analytics
            where user.Account.Role == DiscussionRole.Student
            group user by user.Tutorial.Tutor;

        var heartsOfTutor =
            from grouping in queryTutorGroups
            let hearts = grouping.Sum(student => student.Reactions.Hearts)
            orderby hearts descending
            select (grouping.Key, hearts);

        heartsOfTutor.Take(DefaultCount).ForEachCountedWriteCommaSeparated();
    }

    public void TutorsByEndorsements()
    {
        Analytics
            .GroupByTutor(user => user.Reactions.Endorsements)
            .Take(DefaultCount)
            .WriteEnumeratedCount();
    }

    public void MostHeartedStudents()
    {
        var query = 
            from user in Analytics
            where user.Reactions.Hearts > 20 && user.Account.Role == DiscussionRole.Student
            orderby user.Reactions.Hearts descending
            select new 
            { 
                user.Account.Name, 
                user.Reactions.Hearts 
            };

        foreach ((var i, var poster) in query.Take(UserAnalytics.DefaultCount).Enumerate(start: 1))
            Console.WriteLine($"{i}. {poster.Name}: {poster.Hearts} hearts");
    }

    public void MostVotedStudentsByComments()
    {
        int TotalVotes(User user, Threads threads)
        {
            var votes = 0;
            foreach (var thread in Threads)
            {
                votes += thread
                    .Responses()
                    .Where(response => response.Poster.Name == user.Account.Name)
                    .Sum(response => response.Attributes.Votes);
            }

            return votes;
        }

        Analytics
            .StudentsOnly()
            .CountFormatAndOrderBy(user => TotalVotes(user, Threads), user => user.Account.Name, count: 15)
            .WriteEnumeratedCount();
    }

    public void MostEndorsedStudents()
    {
        var topEndorsed = Analytics.StudentsOnly().TopEndorsed(count: 5);
        foreach ((var i, var user) in topEndorsed.Enumerate(start: 1))
            Console.WriteLine($"{i}. {user.Account.Name} ({user.Reactions.Endorsements})");
    }

    public void TopAcceptedAnswerers()
    {
        Analytics
            .StudentsOnly()
            .Where(user => user.Responses.AcceptedAnswers > 3)
            .OrderByDescending(user => user.Responses.AcceptedAnswers)
            .Enumerate(start: 1)
            .Select(pair =>
                $"{pair.Item1}. {pair.Item2.Account.Name} " +
                $"({pair.Item2.Responses.AcceptedAnswers}/{pair.Item2.Responses.Answers} " +
                $"or {pair.Item2.AcceptedAnswerPercentage()}%)")
            .ForEachWriteLine();
    }

    public void MostVotedThreadsIncludingComments()
    {
        Threads
            .StudentsOnly()
            .MostVotedThreadsIncludingComments()
            .OrderByDescending(thread => thread.TotalVotes())
            .Enumerate(start: 1)
            .Select(pair =>
                $"{pair.Item1}. '{pair.Item2.Title.Text}' {pair.Item2.InternalHyperlink()} " +
                $"by {pair.Item2.PosterProperName()} ({pair.Item2.TotalVotes()} votes)")
            .ForEachWriteLine();
    }

    public void MostVotedThreads()
    {
        Threads
            .StudentsOnly()
            .OrderByDescending(thread => thread.Interactions.Votes)
            .Take(DefaultCount)
            .Enumerate(start: 1)
            .Select(pair =>
                $"{pair.Item1}. '{pair.Item2.Title.Text}' {pair.Item2.InternalHyperlink()} " +
                $"by {pair.Item2.PosterProperName()} ({pair.Item2.Interactions.Votes} votes)")
            .ForEachWriteLine();
    }

    public void CourseCodeOccurrencesByThread(string courseCode)
    {
        Threads
            .Public()
            .CountAndOrderBy(thread => thread.CountOccurrences(courseCode))
            .Select(pair => (pair.Item1.Summary(), pair.Item2))
            .ForEachCountedWriteCommaSeparated();
    }

    public void ThreadsWithMostComments()
    {
        Console.Write("Most commented on threads: ");
        Threads
            .StudentsOnly()
            .Public()
            .CountAndOrderBy(thread => thread.Responses().Count())
            .Select(pair => (pair.Item1.InternalHyperlink(), pair.Item2))
            .ForEachCountedWriteCommaSeparated();
    }

    public void ThreadsWithMostPositiveSentiment()
    {
        var mostPositiveThreads = Threads.Public().SuperlativeSentiment(VaderSentiment.Positive);
        Console.Write("The most positive threads in the discussion are: ");
        mostPositiveThreads
            .Select(thread =>
                $"{thread.InternalHyperlink()} ({thread.Sentiments().AssociatedScore(VaderSentiment.Positive)})")
            .ForEachWriteCommaSeparated();
    }

    public void MostStudentThreadParticipations()
    {
        bool ParticipatedInThread(DiscussionThread thread, string name) =>
            thread.Poster.Name == name 
            || thread.Responses().Any(response => response.Poster.Name == name);

        int ThreadParticipations(User user) =>
            Threads.Count(thread => ParticipatedInThread(thread, user.Account.Name));
        
        Analytics
            .StudentsOnly()
            .CountFormatAndOrderBy(user => ThreadParticipations(user), user => user.Account.Name)
            .WriteEnumeratedCount();
    }

    public void ThreadCountsByHour()
    {
        Threads
            .Where(thread => thread.Metadata.CreationTime is FullTime)
            .Select(thread => ((FullTime) thread.Metadata.CreationTime).DateTime.Hour)
            .GroupBy(hour => hour)
            .Select(pair => (pair.Key.TwentyFourHourToTwelveHour(), pair.Sum()))
            .Where(pair => pair.Item2 != 0)
            .OrderByDescending(pair => pair.Item2)
            .WriteEnumeratedCount();
    }

    public void MostDaysActive()
    {
        Analytics
            .StudentsOnly()
            .CountFormatAndOrderBy(user => user.Participation.DaysActive, user => user.Account.Name)
            .WriteEnumeratedCount();
    }
}