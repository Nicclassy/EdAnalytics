using System.Globalization;

namespace Ed.Analytics.Models;

public sealed record UserParticipation(int Views, int DaysActive, DateTime Enrolled)
{
    private const string DatetimeFormat = "ddd, dd MMM yyyy HH:mm:ss";
    private const string EnrolmentTimezone = "AEST";
    private const int HoursDifference = 10;

    public static UserParticipation Parse(string views, string daysActive, string enrolled)
    {
        string enrolmentWithoutTimezone = 
            enrolled.Replace(EnrolmentTimezone, "").TrimEnd();
        DateTime datetime = DateTime.ParseExact(
            enrolmentWithoutTimezone, 
            DatetimeFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal
        );
        return new(
            int.Parse(views), 
            int.Parse(daysActive), 
            datetime.AddHours(HoursDifference)
        );
    }
}