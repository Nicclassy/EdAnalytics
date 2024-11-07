namespace EdAnalytics.EdDiscussion;

public sealed class Discussion(Analytics _analytics, Threads _threads)
{
    public Analytics Analytics { get; } = _analytics;
    public Threads Threads { get; } = _threads;
}