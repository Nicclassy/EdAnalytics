using VaderSharp;

using Ed.Analytics.Models;

namespace Ed.Analytics.Discussion;

public sealed record ThreadSentiments(
    ThreadSentiment Positive, 
    ThreadSentiment Negative, 
    ThreadSentiment Neutral, 
    ThreadSentiment Compound
)
{
    public double AssociatedScore(VaderSentiment sentiment) =>
        sentiment switch
        {
            VaderSentiment.Positive => Positive.Score,
            VaderSentiment.Negative => Negative.Score,
            VaderSentiment.Neutral => Neutral.Score,
            VaderSentiment.Compound => Compound.Score,
            _ => throw new ArgumentException(nameof(sentiment))
        };
}

public static class ThreadsSentiments
{
    public const int DefaultCount = ThreadsStatistics.DefaultCount;

    public static SentimentIntensityAnalyzer SentimentAnalyzer = new();

    public static ThreadSentiments Sentiments(this DiscussionThread thread) 
    {
        var input = thread.Content.Text
            + thread.Comments.Concat(thread.Answers).Select(comment => comment.Content.Text + ' ');
            
        var scores = SentimentAnalyzer.PolarityScores(input);
        var positive = new ThreadSentiment(scores.Positive, VaderSentiment.Positive);
        var negative = new ThreadSentiment(scores.Negative, VaderSentiment.Negative);
        var neutral = new ThreadSentiment(scores.Neutral, VaderSentiment.Neutral);
        var compound = new ThreadSentiment(scores.Compound, VaderSentiment.Compound);
        return new(positive, negative, neutral, compound);
    }
        
    public static ThreadSentiment PredominantSentiment(this DiscussionThread thread)
    {
        var threadSentiments = thread.Sentiments();
        var sentiments = new ThreadSentiment[4]
        {
            threadSentiments.Positive,
            threadSentiments.Negative,
            threadSentiments.Neutral,
            threadSentiments.Compound
        };

        return sentiments.MaxBy(sentiment => sentiment.Score)!;
    }

    public static Threads WithSentiment(this Threads threads, VaderSentiment sentiment) =>
        new(threads
            .Where(thread => thread.PredominantSentiment().VaderSentiment == sentiment));

    public static Threads SuperlativeSentiment(this Threads threads, VaderSentiment sentiment, int count = DefaultCount) =>
        new(threads
            .OrderByDescending(thread => thread.Sentiments().AssociatedScore(sentiment))
            .Take(count));
}