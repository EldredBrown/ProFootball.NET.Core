using System;
using System.Collections.Generic;
using System.Text;

namespace EldredBrown.ProFootball.Net.Services.Utilities
{
    public class MathUtils
    {
        private const double _exponent = 2.37;

        public static decimal? CalculateExpectedWinningPercentage(decimal pointsFor, decimal pointsAgainst)
        {
            if (pointsFor < 0 || pointsAgainst < 0)
            {
                throw new ArgumentOutOfRangeException($"Points values must be non-negative; got {pointsFor},  {pointsAgainst}.");
            }

            var o = Math.Pow((double)pointsFor, _exponent);
            var d = Math.Pow((double)pointsAgainst, _exponent);
            decimal? result = Divide((decimal)o, (decimal)(o + d));
            return result;
        }

        public static decimal? Divide(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }
    }
}
