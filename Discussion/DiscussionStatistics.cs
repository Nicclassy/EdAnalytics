using Ed.Analytics.Models;
using Ed.Analytics.Common;

namespace Ed.Analytics.Discussion;

public sealed class DiscussionStatistics(UserAnalytics _analytics, Threads _threads)
{
    public UserAnalytics Analytics { get; } = _analytics;
    public Threads Threads { get; } = _threads;

    public void MostHeartedStudents()
    {
        var query = 
            from user in Analytics
            where user.Reactions.Hearts > 20 && user.Account.Role == DiscussionRole.Student
            orderby user.Reactions.Hearts descending
            select new { user.Account.Name, user.Reactions.Hearts };

        foreach ((int i, var poster) in query.Take(3).Enumerate(start: 1))
            Console.WriteLine($"{i}. {poster.Name}: {poster.Hearts} hearts");
        Console.WriteLine();
    }

    public void TopEndorsedStudents()
    {
        var topEndorsed = Analytics.StudentsOnly().TopEndorsed(count: 5);
        foreach ((int i, User user) in topEndorsed.Enumerate(start: 1))
            Console.WriteLine($"{i}. {user.Account.Name} ({user.Reactions.Endorsements})");
        Console.WriteLine();
    }

    public void TopAcceptedAnswerers()
    {
        Analytics
            .StudentsOnly()
            .Where(user => user.Responses.AcceptedAnswers > 3)
            .OrderByDescending(user => user.Responses.AcceptedAnswers)
            .Enumerate(start: 1)
            .Select(pair =>
                $"{pair.Item1}. {pair.Item2.Account.Name} ({pair.Item2.Responses.AcceptedAnswers}/{pair.Item2.Responses.Answers} or {pair.Item2.AcceptedAnswerPercentage()}%)"
            )
            .ForEachWriteLine();
        Console.WriteLine();
    }

    public void DisplayMostVotedThreads()
    {
        Threads
            .StudentsOnly()
            .MostVotedThreadsIncludingComments()
            .OrderByDescending(thread => thread.TotalVotes())
            .Enumerate(start: 1)
            .Select(pair =>
                $"{pair.Item1}. '{pair.Item2.Title.Text}' by {pair.Item2.Poster.Name} ({pair.Item2.TotalVotes()} votes)"
            )
            .ForEachWriteLine();
        Console.WriteLine();
    }

    public void CourseCodeOccurrences(string courseCode)
    {
        Threads
            .Public()
            .CountAndOrderBy(thread => thread.CountOccurrences(courseCode))
            .Select(pair => (pair.Item1.Summary(), pair.Item2))
            .ForEachCountedWriteCommaSeparated();
        Console.WriteLine();
    }

    public void PublicAndNotAnonymous()
    {
        var studentThreads = Threads.StudentsOnly();
        int threadCount = studentThreads.Length;
        var notPrivateOrAnonymous = studentThreads.Count(thread => 
            !thread.Attributes.Anonymous && !thread.Attributes.Private
        );
        Console.WriteLine($"{notPrivateOrAnonymous.PercentageOf(threadCount)}% of all threads were public and not anonymous");
        Console.WriteLine();
    }

    public void GratitudePercentage()
    {
        var studentThreads = Threads.StudentsOnly();
        int threadCount = studentThreads.Length;
        var thankYous = studentThreads.Count(thread =>
        {
            var thankYou = thread.CountOccurrences("Thank you");
            var thanks = thread.CountOccurrences("Thanks");
            return thankYou + thanks > 0;
        });
        Console.WriteLine($"{thankYous.PercentageOf(threadCount)}% had someone express gratitude saying either 'Thanks' or 'Thank you'");
        Console.WriteLine();
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
        Console.WriteLine();
    }

    public void ThreadsWithMostPositiveSentiment()
    {
        var mostPositiveThreads = Threads.SuperlativeSentiment(VaderSentiment.Positive);
        Console.Write("The most positive threads in the discussion are: ");
        mostPositiveThreads
            .Select(thread =>
                $"{thread.InternalHyperlink()} ({thread.Sentiments().AssociatedScore(VaderSentiment.Positive)})"
            )
            .ForEachWriteCommaSeparated();
        Console.WriteLine();
    }

    public void CategoryResponseRate(string categoryName)
    {
        var assignmentOneThreads = Threads.Questions().WithCategory(categoryName);
        var respondedThreadsCount = assignmentOneThreads.Count(thread => thread.IsRespondedTo());
        var percentageAnswered = respondedThreadsCount.PercentageOf(assignmentOneThreads.Count());
        Console.WriteLine($"{percentageAnswered}% of threads with category '{categoryName}' were answered");
        Console.WriteLine();
    }
}