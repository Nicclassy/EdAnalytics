namespace EdAnalytics.Models;

public readonly record struct CommentAttributes(int Votes, bool Endorsed, bool Anonymous)
{
    public CommentAttributes(int? votes, bool? endorsed, bool? anonymous) :
        this(votes ?? default, endorsed ?? default, anonymous ?? default) {}
}