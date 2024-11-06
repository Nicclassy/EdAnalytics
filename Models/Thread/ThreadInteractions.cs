namespace EdAnalytics.Models;

public readonly record struct ThreadInteractions(int Votes, int Views, int UniqueViews) 
{
    public ThreadInteractions(int? votes, int? views, int? uniqueViews) :
        this(votes ?? default, views ?? default, uniqueViews ?? default) {}
}