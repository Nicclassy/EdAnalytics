namespace Ed.Analytics.Models;

public enum VaderSentiment { Positive, Negative, Neutral, Compound }

public sealed record ThreadSentiment(double Score, VaderSentiment VaderSentiment) : IComparable<ThreadSentiment>
{
    public int CompareTo(ThreadSentiment? other) => Score.CompareTo(other?.Score);
}