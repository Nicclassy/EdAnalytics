using System.Collections.Immutable;
using Microsoft.VisualBasic.FileIO;

using EdAnalytics.Models;
using EdAnalytics.EdDiscussion;

namespace EdAnalytics.Deserialization;

public static class AnalyticsCSV
{
    public static Analytics Parse(string path) => Parse(path, Info1112Tutorial.Create);

    public static Analytics Parse(string path, TutorialFactory factory)
    {
        using TextFieldParser parser = new(path);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.ReadLine();

        var analytics = ImmutableArray.CreateBuilder<User>();
        while (!parser.EndOfData)
        {
            var fields = 
                parser.ReadFields() ?? throw new InvalidDataException("Invalid fields");
            analytics.Add(ParseUserAnalytics(fields, factory));
        }

        return new(analytics.ToImmutable());
    }

    private static User ParseUserAnalytics(string[] fields, TutorialFactory factory)
    {
        var user = Account.Parse(fields[0], fields[1], fields[2]);
        var tutorial = factory.Invoke(fields[3]);
        var threads = new UserThreads(
            int.Parse(fields[6]),
            int.Parse(fields[7]),
            int.Parse(fields[8])
        );
        var responses = new UserResponses(
            int.Parse(fields[9]),
            int.Parse(fields[10]),
            int.Parse(fields[11])
        );
        var reactions = new FavourableReactions(
            int.Parse(fields[12]),
            int.Parse(fields[13])
        );
        var discussion = UserDiscussionStatistics.Create(
            fields[5],
            fields[16],
            fields[18]
        );
        return new(
            user,
            tutorial,
            threads,
            responses,
            reactions,
            discussion
        );
    }
}