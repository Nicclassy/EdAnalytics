namespace Ed.Analytics.EdDiscussion;

public sealed class Discussion(UserAnalytics _analytics, Threads _threads)
{
    public UserAnalytics Analytics { get; } = _analytics;
    public Threads Threads { get; } = _threads;
}