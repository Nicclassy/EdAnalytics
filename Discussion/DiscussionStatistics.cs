using Ed.Analytics.Models;
using Ed.Analytics.Common;
using System.Text.RegularExpressions;

namespace Ed.Analytics.Discussion;

public sealed class DiscussionStatistics(UserAnalytics _analytics, Threads _threads)
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

    public void TopEndorsedStudents()
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
                $"{pair.Item1}. '{pair.Item2.Title.Text}' " +
                $"by {pair.Item2.Poster.Name} ({pair.Item2.TotalVotes()} votes)"
            )
            .ForEachWriteLine();
    }

    public void CourseCodeOccurrences(string courseCode)
    {
        Threads
            .Public()
            .CountAndOrderBy(thread => thread.CountOccurrences(courseCode))
            .Select(pair => (pair.Item1.Summary(), pair.Item2))
            .ForEachCountedWriteCommaSeparated();
    }

    public void PublicAndNotAnonymous()
    {
        var studentThreads = Threads.StudentsOnly();
        int threadCount = studentThreads.Length;
        var notPrivateOrAnonymous = studentThreads.Count(thread => 
            !thread.Attributes.Anonymous && !thread.Attributes.Private
        );
        Console.WriteLine(
            $"{notPrivateOrAnonymous.PercentageOf(threadCount)}% " +
            "of all threads were public and not anonymous");
    }

    public void GratitudePercentage()
    {
        var studentThreads = Threads.StudentsOnly();
        int threadCount = studentThreads.Length;
        var thankYous = studentThreads.Count(delegate(DiscussionThread thread)
        {
            var thankYou = thread.CountOccurrences("Thank you");
            var thanks = thread.CountOccurrences("Thanks");
            return thankYou + thanks > 0;
        });
        Console.WriteLine(
            $"{thankYous.PercentageOf(threadCount)}% " +
            "had someone express gratitude saying either 'Thanks' or 'Thank you'");
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
        var mostPositiveThreads = Threads.SuperlativeSentiment(VaderSentiment.Positive);
        Console.Write("The most positive threads in the discussion are: ");
        mostPositiveThreads
            .Select(thread =>
                $"{thread.InternalHyperlink()} ({thread.Sentiments().AssociatedScore(VaderSentiment.Positive)})")
            .ForEachWriteCommaSeparated();
    }

    public void CategoryResponseRate(string categoryName)
    {
        var a1Threads = Threads.WithCategory(categoryName).Questions().StudentsOnly();
        var responseCount = a1Threads.Count(thread => thread.IsRespondedTo());
        var percentageAnswered = responseCount.PercentageOf(a1Threads.Count());
        Console.WriteLine($"{percentageAnswered}% of threads with category '{categoryName}' were answered");
    }

    public void CommentsButNoAnswers(string categoryName)
    {
        var selection = Threads.WithCategory(categoryName).Questions().StudentsOnly();
        var count = selection
            .Where(thread => thread.Comments.Any() && !thread.Answers.Any())
            .Count();
        Console.WriteLine(
            $"{count.PercentageOf(selection.Length)}% " +
            $"of threads with category '{categoryName}' were commented on but had no answers");
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

    public void PercentageOfUsersWithEmojis()
    {
        var totalCount = Analytics.Length;
        var withEmoji = Analytics.Where(user => user.Account.Name.ContainsEmoji()).Count();
        var percentage = withEmoji.PercentageOf(totalCount);
        Console.WriteLine(
            $"{percentage}% of users ({withEmoji} users) " +
            "in the INFO1112 discussion have an emoji in their name");
    }

    public void NumberOfThreadHyperlinks()
    {
        int HyperlinkCount(string text) => Regex.Matches(text, @"#\d+").Count;
        int ThreadHyperlinkCount(DiscussionThread thread)
        {
            var count = HyperlinkCount(thread.Content.Text);
            count += thread.Responses().Sum(response => HyperlinkCount(thread.Content.Text));
            return count;
        }

        var hyperlinkCount = Threads.Sum(ThreadHyperlinkCount);
        Console.WriteLine($"{hyperlinkCount} hyperlinks were used");
    }

    public void AverageStudentResponseTime()
    {
        var studentThreads = Threads.StudentsOnly().Questions();
        var responseTime = studentThreads.AverageResponseTime(DiscussionRole.Student);
        Console.WriteLine(
            "Of all threads responded to by students, " +
            "the average response time of the fastest response was " +
            $"{responseTime.Hours} hours and {responseTime.Minutes} minutes");
    }

    public void AverageTutorResponseTime()
    {
        var studentThreads = Threads.StudentsOnly().Questions();
        var responseTime = studentThreads.AverageResponseTime(DiscussionRole.Staff, DiscussionRole.Admin);
        Console.WriteLine(
            "Of all threads responded to by tutors, " +
            "the average response time of the fastest response was " +
            $"{responseTime.Hours} hours and {responseTime.Minutes} minutes");
    }
}