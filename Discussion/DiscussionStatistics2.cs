using System.Text.RegularExpressions;

using Ed.Analytics.Common;
using Ed.Analytics.Models;

namespace Ed.Analytics.Discussion;

public partial class DiscussionStatistics
{
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

    public void PercentageOfUsersWithEmojis()
    {
        var totalCount = Analytics.Length;
        var withEmoji = Analytics.Where(user => user.Account.Name.ContainsEmoji()).Count();
        var percentage = withEmoji.PercentageOf(totalCount);
        Console.WriteLine(
            $"{percentage}% of users ({withEmoji} users) " +
            "in the INFO1112 discussion have an emoji in their name");
    }

    public void AverageResponseTime()
    {
        var studentThreads = Threads.StudentsOnly().Questions();
        var responseTime = studentThreads.AverageResponseTime();
        Console.WriteLine(
            "The average response time of the fastest response was " +
            $"{responseTime.Hours} hours and {responseTime.Minutes} minutes for all threads");
    }

    public void AverageStudentResponseTime()
    {
        var threads = Threads.StudentsOnly().Questions();
        var responseTime = threads.AverageResponseTime(DiscussionRole.Student);
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

    public void PercentageAnsweredMoreThanPosted()
    {
        bool AnsweredMoreThanPosted(User user) =>
            user.Responses.Answers > user.Threads.Posts + user.Threads.Questions;

        var students = Analytics.StudentsOnly();
        var answeredMoreThanPosted = students.Where(AnsweredMoreThanPosted).Count();
        var percentage = answeredMoreThanPosted.PercentageOf(students.Length);
        Console.WriteLine($"{percentage}% of students answered threads more than they posted them");
    }

    public void PercentageMoreAnonymousThanNot()
    {
        (int, int) CountAnonymousThreads(Threads threads, User user)
        {
            var (anonymous, nonAnonymous) = (0, 0);
            foreach (var thread in threads)
            {
                if (thread.Poster.Name == user.Account.Name)
                {
                    if (thread.Attributes.Anonymous) anonymous++;
                    else nonAnonymous++;
                }
            }

            return (anonymous, nonAnonymous);
        }

        var students = Analytics.StudentsOnly();
        var studentsWithMoreAnonymousThreads = students
            .Where(student => 
            {
                var (anonymous, nonAnonymous) = CountAnonymousThreads(Threads.StudentsOnly(), student);
                if (anonymous == 0 && nonAnonymous == 0) return false;
                return anonymous > nonAnonymous;
            })
            .Count();
        var percentage = studentsWithMoreAnonymousThreads.PercentageOf(students.Length);
        Console.WriteLine($"{percentage}% of students created more anonymous threads than non-anonymous threads");
    }

    public void PercentageNeverMadeThread()
    {
        bool NeverMadeThread(User user) => user.Threads.Posts == 0 && user.Threads.Questions == 0;

        var students = Analytics.StudentsOnly();
        var noThreads = students.Count(NeverMadeThread);
        var percentage = noThreads.PercentageOf(students.Length);
        Console.WriteLine($"{percentage}% of students have never made a thread");
    }

    public void PercentageParticipatedEveryDayOfTheWeek()
    {
        var everyDay = 0b1111111;
        int DayOfWeekValue(CreationTime creationTime)
        {
            var dayOfWeek = 0;
            if (creationTime is FullTime(DateTime dateTime))
            {
                dayOfWeek = 1 << ((int) dateTime.DayOfWeek);
            }
            return dayOfWeek;
        }

        bool ParticipatedEveryDay(User user, Threads threads)
        {
            var daysParticipated = 0b0;
            foreach (var thread in threads)
            {
                if (thread.Poster == user.Account)
                {
                    var weekday = DayOfWeekValue(thread.Metadata.CreationTime);
                    daysParticipated |= weekday;
                }
                
                var userResponses = thread
                    .Responses()
                    .Where(response => response.Poster == user.Account);
                foreach (var response in userResponses) 
                {
                    var weekday = DayOfWeekValue(thread.Metadata.CreationTime);
                    daysParticipated |= weekday;
                }
            }

            return daysParticipated == everyDay;
        }

        var students = Analytics.StudentsOnly();
        var everyDayParticipants = students
            .Where(user => ParticipatedEveryDay(user, Threads))
            .Count();
        var percentage = everyDayParticipants.PercentageOf(students.Length);
        Console.WriteLine(
            $"{percentage}% of students participated in at least one thread on every day of the week, " +
            "meaning they either created or responded to a thread on each day from Monday to Sunday");
    }

    public void CourseCodeOccurrences(string courseCode)
    {
        var courseCodeOccurrences = Threads
            .Public()
            .Sum(thread => thread.CountOccurrences(courseCode));
        Console.WriteLine($"The course code '{courseCode}' appeared {courseCodeOccurrences} times");
    }
}