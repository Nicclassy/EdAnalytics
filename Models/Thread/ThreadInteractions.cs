namespace EdAnalytics.Models;

public sealed class ThreadInteractions(int votes, int views, int uniqueViews) 
{
    public int Votes { get; } = votes;
    public int Views { get; } = views;
    public int UniqueViews { get; } = uniqueViews;

    public ThreadInteractions(int? votes, int? views, int? uniqueViews) :
        this(votes ?? default, views ?? default, uniqueViews ?? default) {}
}