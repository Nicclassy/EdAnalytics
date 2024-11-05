namespace EdAnalytics.Models;

public sealed class ThreadInteractions(int _votes, int _views, int _uniqueViews) 
{
    public int Votes { get; } = _votes;
    public int Views { get; } = _views;
    public int UniqueViews { get; } = _uniqueViews;

    public ThreadInteractions(int? votes, int? views, int? uniqueViews) :
        this(votes ?? default, views ?? default, uniqueViews ?? default) {}
}