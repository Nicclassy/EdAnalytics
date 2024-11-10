using System.Numerics;

namespace Ed.Analytics.Common;

public static class NumberExtensions
{
    public static double PercentageOf<T, U>(this T part, U whole) where T: INumber<T> where U: INumber<U> =>
        whole == U.Zero ? 0 : Math.Round(Convert.ToDouble(part) / Convert.ToDouble(whole) * 100, 2);

    public static string TwentyFourHourToTwelveHour(this int hour) => hour > 12 ? $"{hour - 12}pm" : $"{hour}am";
}