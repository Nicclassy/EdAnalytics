namespace EdAnalytics.Models;

public sealed record CommentAttributes(int Votes, bool Endorsed, bool Anonymous)
{
    public CommentAttributes(int? votes, bool? endorsed, bool? anonymous) :
        this(votes ?? default, endorsed ?? default, anonymous ?? default) {}
}