namespace Ed.Analytics.Models;

public enum DayAbbreviated { Mon, Tue, Wed, Thur, Fri, Sat, Sun }

public delegate Tutorial TutorialFactory(string value);

public abstract class Tutorial
{
    public abstract int Lab { get; }
    public abstract int StartHour { get; }
    public abstract DayAbbreviated Day { get; }
    public abstract string Room { get; }
    public abstract string Tutor { get; }
}

public sealed class Info1112Tutorial(string[] tags) : Tutorial
{
    private const char TagsDelimiter = ';';

    public override int Lab => TagParser.Lab(tags[0]);
    public override int StartHour => int.Parse(tags[2]);
    public override DayAbbreviated Day => TagParser.Day(tags[1]);
    public override string Room => tags[3];
    public override string Tutor => tags[4];

    public static Info1112Tutorial Create(string value) => 
        new(value.Split(TagsDelimiter));
}

public static class TagParser
{
    public static int Lab(string value) => 
        int.Parse(value.Substring(3, 2));
    
    public static DayAbbreviated Day(string value) =>
        Enum.Parse<DayAbbreviated>(value);
}