using System.Collections;
using System.Collections.Immutable;

using Ed.Analytics.Models;

namespace Ed.Analytics.Discussion;

public delegate IComparable ThreadSelector(DiscussionThread thread);

public sealed class Threads(ImmutableArray<DiscussionThread> _threads) : IEnumerable<DiscussionThread>
{
    public int Length => _threads.Length;

    private const int SearchTolerance = 10;

    public Threads(IEnumerable<DiscussionThread> threads) : this(threads.ToImmutableArray()) {}

    public DiscussionThread this[Index index] => _threads[index];

    public IEnumerator<DiscussionThread> GetEnumerator() =>
        _threads.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public DiscussionThread WithNumber(int threadNumber)
    {
        // Search with a level of tolerance because a thread might be missing and
        // not directly in the index that is used to search.
        if (threadNumber < 1 || threadNumber > _threads.Length)
            throw new ArgumentOutOfRangeException(nameof(threadNumber));

        var initialTarget = _threads[threadNumber - 1];
        var targetThreadNumber = initialTarget.Metadata.Number;
        if (targetThreadNumber == threadNumber)
            return initialTarget;

        var searches = 1;
        var threadNumberIsHigher = targetThreadNumber.Value > threadNumber;

        var low = threadNumberIsHigher ? 0 : threadNumber - 1;
        var high = threadNumberIsHigher ? threadNumber - 2 : _threads.Length - 1;
        // Since we are searching via index, minus one from either side of the search
        while (low <= high && searches < SearchTolerance)
        {
            var mid = low + (high - low) / 2;
            var thread = _threads[mid];
            if (thread.Metadata.Number == mid)
                return thread;

            if (thread.Metadata.Number.Value > mid)
                low = mid + 1;
            else
                high = mid - 1;
            searches++;
        }

        throw new KeyNotFoundException("a thread with the given number was not found");
    }
}

public static class ThreadsFiltering
{
    public static IEnumerable<(DiscussionThread, IComparable)> CountAndOrderBy(
        this Threads threads, 
        ThreadSelector selector,
        int count = ThreadsStatistics.DefaultCount
    ) => threads
            .Select(thread => (thread, selector(thread)))
            .OrderByDescending(pair => pair.Item2)
            .Take(count);

    public static Threads Questions(this Threads threads) =>
        new(threads
            .Where(thread => thread.Metadata.Type == ThreadType.Question));

    public static Threads WithCategory(this Threads threads, string categoryName) =>
        new(threads
            .Where(thread => thread.Categories.Category is Category(string name) && name.Equals(categoryName)));

    public static Threads StudentsOnly(this Threads threads) => 
        new(threads
            .Where(thread => thread.Poster.Role == DiscussionRole.Student));

    public static Threads Public(this Threads threads) =>
        new(threads
            .Where(thread => !thread.Attributes.Private));

    public static Threads OfUser(this Threads threads, Account user) => threads.OfUser(user.Name);

    public static Threads OfUser(this Threads threads, string username) =>
        new(threads
            .Where(thread => thread.Poster.Name == username).ToImmutableArray());
}

public static class ThreadsStatistics
{
    public const int DefaultCount = 5;

    public static Threads MostVotedThreads(this Threads threads, int count = DefaultCount) =>
        new(threads
            .OrderByDescending(thread => thread.Interactions.Votes)
            .Take(count));

    public static Threads MostVotedThreadsIncludingComments(this Threads threads, int count = DefaultCount) => 
        new(threads 
            .OrderByDescending(thread => thread.TotalVotes())
            .Take(count));
}

public static class ThreadStatistics
{
    public static int TotalVotes(this DiscussionThread thread) 
    {
        var threadVotes = thread.Interactions.Votes;
        var answerVotes = thread.Answers.Sum(VotesIncludingSubcomments);
        var commentVotes = thread.Comments.Sum(VotesIncludingSubcomments);
        return threadVotes + answerVotes + commentVotes;
    }

    public static int VotesIncludingSubcomments(this Comment comment) =>
        comment.Attributes.Votes + comment.Comments.Sum(VotesIncludingSubcomments);
}