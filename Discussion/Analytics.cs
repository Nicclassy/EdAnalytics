using System.Collections;
using System.Collections.Immutable;

using EdAnalytics.Models;

namespace EdAnalytics.EdDiscussion;

public sealed class Analytics(ImmutableArray<UserAnalytics> analytics) : IEnumerable<UserAnalytics>
{
    public IEnumerator<UserAnalytics> GetEnumerator() =>
        analytics.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}