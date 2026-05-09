using System;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    internal class ProcessGameUtils
    {
        public static decimal? CalculateExpectedWinningPercentage(decimal pointsFor, decimal pointsAgainst)
        {
            double exponent = 2.37d;

            if (pointsFor < 0 || pointsAgainst < 0)
            {
                throw new ArgumentOutOfRangeException($"Points values must be non-negative; got {pointsFor},  {pointsAgainst}.");
            }

            var o = Math.Pow((double)pointsFor, exponent);
            var d = Math.Pow((double)pointsAgainst, exponent);
            decimal? result = Divide((decimal)o, (decimal)(o + d));

            return result;
        }

        private static decimal? Divide(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }
    }
}
