using Microsoft.VisualBasic.FileIO;

using EdAnalytics.Models;

namespace EdAnalytics.Deserialization;

public static class AnalyticsCSV
{
    public static List<UserAnalytics> Parse(string path) => Parse(path, NamedTutorial.Strategy);

    public static List<UserAnalytics> Parse(string path, TutorialStrategy strategy)
    {
        using TextFieldParser parser = new(path);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.ReadLine();

        var analytics = new List<UserAnalytics>();
        while (!parser.EndOfData)
        {
            var fields = 
                parser.ReadFields() ?? throw new InvalidDataException("Invalid fields");
            analytics.Add(ParseUserAnalytics(fields, strategy));
        }

        return analytics;
    }

    private static UserAnalytics ParseUserAnalytics(string[] fields, TutorialStrategy strategy)
    {
        var user = User.Parse(fields[0], fields[1], fields[2]);
        var tutorial = strategy.Invoke(fields[3]);
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
        var discussion = DiscussionStatistics.Create(
            fields[5],
            fields[16],
            fields[18]
        );
        return new UserAnalytics(
            user,
            tutorial,
            threads,
            responses,
            reactions,
            discussion
        );
    }
}