namespace EdAnalytics.Models;

public enum WeekdayAbbreviated { Mon, Tue, Wed, Thur, Fri, Sat, Sun }

public delegate Tutorial TutorialStrategy(string value);

public abstract record Tutorial;

public sealed record NamedTutorial(string Name) : Tutorial
{
    public static NamedTutorial Strategy(string value) => 
        new(value);
}

public sealed record Info1112Tags(string[] Tags) : Tutorial
{
    public int Lab => TagStrategies.Lab(Tags[0]);
    public WeekdayAbbreviated Weekday = TagStrategies.Weekday(Tags[1]);
    public int StartHour => int.Parse(Tags[2]);
    public string Room => Tags[3];
    public string Tutor => Tags[4];

    private const char Delimiter = ';';

    public static Info1112Tags Stategy(string value) =>
        new Info1112Tags(value.Split(Delimiter));
}

public static class TagStrategies
{
    public static int Lab(string value) => 
        int.Parse(value.Substring(3, 2));
    
    public static WeekdayAbbreviated Weekday(string value) =>
        Enum.Parse<WeekdayAbbreviated>(value);
}