using System.Collections.Immutable;
using System.Text.Json;

using EdAnalytics.Models;

namespace EdAnalytics.Deserialization;

public sealed class ThreadsJSONDeserializer 
{
    public static List<DiscussionThread> Deserialize(string path)
    {
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var deserializedThreads = JsonSerializer.Deserialize<List<DeserializedThread>>(json, options)!;
        return deserializedThreads.Select(ParseDiscussionThread).ToList();
    }

    private static DiscussionThread ParseDiscussionThread(DeserializedThread thread)
    {
        var user = 
            thread.User ?? throw new InvalidDataException("User not found in thread");
        var poster = User.Parse(
            user.Name, 
            user.Email, 
            user.Role
        );
        var metadata = ThreadMetadata.Parse(
            thread.Url, thread.CreatedAt,
            thread.Type, thread.Number
        );
        var categories = new ThreadCategories(
            thread.Category,
            thread.Subcategory,
            thread.Subsubcategory
        );
        var title = new ContentText(
            thread.Title ?? 
            throw new InvalidDataException("Thread must have a title")
        );
        var text = new ContentText(
            thread.Text ??
            throw new InvalidDataException("Thread must have text")
        );
        var interactions = new ThreadInteractions(
            thread.Votes,
            thread.Views,
            thread.UniqueViews
        );
        var attributes = new ThreadAttributes(
            thread.Private,
            thread.Anonymous,
            thread.Endorsed
        );
        
        return new DiscussionThread(
            poster,
            title,
            text,
            metadata,
            categories,
            interactions,
            attributes,
            ParseComments(thread.Answers ?? []),
            ParseComments(thread.Comments ?? [])
        );
    }

    private static ImmutableArray<Comment> ParseComments(List<DeserializedComment> comments) =>
        comments.Select(ParseComment).ToImmutableArray();

    private static Comment ParseComment(DeserializedComment comment)
    {
        var user = 
            comment.User ?? throw new InvalidDataException("User not found in thread");
        var poster = User.Parse(
            user.Name, 
            user.Email, 
            user.Role
        );
        var url = ContentURL.Parse(comment.Url);
        var attributes = new CommentAttributes(
            comment.Votes,
            comment.Endorsed,
            comment.Anonymous
        );
        var creationDate = CreationDate.Parse(comment.CreatedAt);
        var text = new ContentText(
            comment.Text
            ?? throw new InvalidDataException("Comment text not provided")
        );
        var metadata = new CommentMetadata(url, creationDate);
        var nestedComments = (comment.Comments ?? []).Any() 
            ? ParseComments(comment.Comments!)
            : ImmutableArray<Comment>.Empty;

        return new Comment(
            poster,
            text,
            metadata,
            attributes,
            nestedComments
        );
    }
}

class DeserializedUser
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}

class DeserializedComment
{
    public string? Url { get; set; }
    public int? Votes { get; set; }
    public string? CreatedAt { get; set; }
    public DeserializedUser? User { get; set; }

    public string? Text { get; set; }
    public string? Document { get; set; }

    public bool? Endorsed { get; set; }
    public bool? Anonymous { get; set; }
    public List<DeserializedComment>? Comments { get; set; }
}

class DeserializedThread
{
    public string? Url { get; set; }
    public string? Type { get; set; }
    public int? Number { get; set; }

    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Subcategory { get; set; }
    public string? Subsubcategory { get; set; }

    public int? Votes { get; set; }
    public int? Views { get; set; }
    public int? UniqueViews { get; set; }

    public bool? Private { get; set; }
    public bool? Anonymous { get; set; }
    public bool? Endorsed { get; set; }
    public string? CreatedAt { get; set; }

    public DeserializedUser? User { get; set; }
    public string? Text { get; set; }
    public string? Document { get; set; }

    public List<DeserializedComment>? Comments { get; set; }
    public List<DeserializedComment>? Answers { get; set; }
}