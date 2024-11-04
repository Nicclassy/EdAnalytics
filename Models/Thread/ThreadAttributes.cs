namespace EdAnalytics.Models;

public sealed record ThreadAttributes(bool Private, bool Anonymous, bool Endorsed)
{
    public ThreadAttributes(bool? private_, bool? anonymous, bool? endorsed) :
        this(private_ ?? default, anonymous ?? default, endorsed ?? default) {}
}