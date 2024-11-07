using System.Collections;
using System.Collections.Immutable;

using Ed.Analytics.Models;

namespace Ed.Analytics.EdDiscussion;

public sealed class Threads(ImmutableArray<DiscussionThread> _threads) : IEnumerable<DiscussionThread>
{
    private const int SearchTolerance = 10;

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

public static class ThreadsExtensions
{
    public static Threads OfUser(this Threads threads, Account user) => threads.OfUser(user.Name);

    public static Threads OfUser(this Threads threads, string username) =>
        new(threads
            .Where(thread => thread.Poster.Name == username).ToImmutableArray());
}