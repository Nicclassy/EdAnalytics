namespace EdAnalytics.Models;

public readonly record struct ThreadAttributes(bool Private, bool Anonymous, bool Endorsed)
{
    public ThreadAttributes(bool? private_, bool? anonymous, bool? endorsed) :
        this(private_ ?? default, anonymous ?? default, endorsed ?? default) {}
}