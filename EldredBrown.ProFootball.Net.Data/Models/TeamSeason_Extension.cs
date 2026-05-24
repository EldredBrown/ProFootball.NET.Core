using System;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    public partial class TeamSeason : ITeamSeason
    {
        private const double _exponent = 2.37;

        public decimal? WinningPercentage
        {
            get
            {
                return Divide(2 * Wins + Ties, 2 * Games);
            }
        }

        /// <summary>
        /// Calculates and updates this <see cref="TeamSeason"/> model's expected wins and losses.
        /// </summary>
        public void CalculateExpectedWinsAndLosses()
        {
            if (Games == 0)
            {
                ExpectedWins = 0m;
                ExpectedLosses = 0m;
                return;
            }

            var expPct = CalculateExpectedWinningPercentage(PointsFor, PointsAgainst);

            if (expPct.HasValue)
            {
                ExpectedWins = expPct.Value * Games;
                ExpectedLosses = (1m - expPct.Value) * Games;
            }
            else
            {
                ExpectedWins = 0;
                ExpectedLosses = 0;
            }
        }

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for this <see cref="TeamSeason"/> object.
        /// </summary>
        /// <param name="teamSeasonScheduleAveragePointsFor"></param>
        /// <param name="teamSeasonScheduleAveragePointsAgainst"></param>
        /// <param name="leagueSeasonAveragePoints"></param>
        public void UpdateRankings(
            decimal teamSeasonScheduleAveragePointsFor,
            decimal teamSeasonScheduleAveragePointsAgainst,
            decimal leagueSeasonAveragePoints)
        {
            var offense = UpdateRankings(PointsFor, Games, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);
            OffensiveAverage = offense.Average;
            OffensiveFactor = offense.Factor;
            OffensiveIndex = offense.Index;

            var defense = UpdateRankings(PointsAgainst, Games, teamSeasonScheduleAveragePointsFor,
                leagueSeasonAveragePoints);
            DefensiveAverage = defense.Average;
            DefensiveFactor = defense.Factor;
            DefensiveIndex = defense.Index;

            CalculateFinalExpectedWinningPercentage();
        }

        private decimal? CalculateExpectedWinningPercentage(decimal pointsFor, decimal pointsAgainst)
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

        private void CalculateFinalExpectedWinningPercentage()
        {
            if (OffensiveIndex is null || DefensiveIndex is null)
            {
                FinalExpectedWinningPercentage = null;
                return;
            }

            FinalExpectedWinningPercentage = CalculateExpectedWinningPercentage(
                OffensiveIndex.Value, DefensiveIndex.Value);
        }

        private decimal? Divide(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }

        private TeamSeasonRankingsData UpdateRankings(int points, int games,
            decimal teamSeasonScheduleAveragePoints, decimal leagueSeasonAveragePoints)
        {
            if (games == 0)
            {
                return new TeamSeasonRankingsData(average: null, factor: null, index: null);
            }

            decimal? average = Divide(points, games);
            decimal? factor = Divide(average!.Value, teamSeasonScheduleAveragePoints);
            decimal? index = factor.HasValue
                ? (average.Value + factor.Value * leagueSeasonAveragePoints) / 2m
                : null;

            return new TeamSeasonRankingsData(average, factor, index);
        }

        private class TeamSeasonRankingsData
        {
            public TeamSeasonRankingsData(decimal? average, decimal? factor, decimal? index)
            {
                Average = average;
                Factor = factor;
                Index = index;
            }

            public decimal? Average { get; set; }
            public decimal? Factor { get; set; }
            public decimal? Index { get; set; }
        }
    }
}
