using System.Collections;
using System.Collections.Immutable;

using EdAnalytics.Models;

namespace EdAnalytics.EdDiscussion;

public sealed class Threads(ImmutableArray<DiscussionThread> threads) : IEnumerable<DiscussionThread>
{
    public IEnumerator<DiscussionThread> GetEnumerator() =>
        threads.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}