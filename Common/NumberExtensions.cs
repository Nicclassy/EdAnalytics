using System.Numerics;

namespace Ed.Analytics.Common;

public static class NumberExtensions
{
    public static double PercentageOf<T, U>(this T whole, U part) where T: INumber<T> where U: INumber<U> =>
        Math.Round(Convert.ToDouble(whole) / Convert.ToDouble(part) * 100, 2);
}