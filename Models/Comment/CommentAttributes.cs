namespace EdAnalytics.Models;

public record CommentAttributes(int votes, bool endorsed, bool anonymous)
{
    public int Votes { get; } = votes;
    public bool Endorsed { get; } = endorsed;
    public bool Anonymous { get; } = anonymous;

    public CommentAttributes(int? votes, bool? endorsed, bool? anonymous) :
        this(votes ?? default, endorsed ?? default, anonymous ?? default) {}
}