using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.ModelTests
{
    public class TeamSeasonTest
    {
        private const double _exponent = 2.37;

        public TeamSeasonTest() { }


        [Fact]
        public void Constructor_ShouldInitializePropertiesFromTeamSeason()
        {
            // Arrange
            var games = 2;
            var wins = 1;
            var losses = 1;
            var ties = 0;
            var pointsFor = 40;
            var pointsAgainst = 40;

            // Act
            var teamSeason = new TeamSeason
            {
                Games = games,
                Wins = wins,
                Losses = losses,
                Ties = ties,
                PointsFor = pointsFor,
                PointsAgainst = pointsAgainst
            };

            // Assert
            teamSeason.Games.ShouldBe(games);
            teamSeason.Wins.ShouldBe(wins);
            teamSeason.Losses.ShouldBe(losses);
            teamSeason.Ties.ShouldBe(ties);
            teamSeason.PointsFor.ShouldBe(pointsFor);
            teamSeason.PointsAgainst.ShouldBe(pointsAgainst);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, null)]
        [InlineData(2, 2, 0, 0, 1.000d)]
        [InlineData(2, 1, 1, 0, 0.500d)]
        [InlineData(2, 0, 2, 0, 0.000d)]
        [InlineData(2, 1, 0, 1, 0.750d)]
        [InlineData(2, 0, 1, 1, 0.250d)]
        [InlineData(2, 0, 0, 2, 0.500d)]
        public void WinningPercentage_ShouldReturnCorrectValue(int games, int wins, int losses, int ties, double? expected)
        {
            // Arrange
            var teamSeason = new TeamSeason
            {
                Games = games,
                Wins = wins,
                Losses = losses,
                Ties = ties
            };

            // Act
            double? result = (double?)(teamSeason.WinningPercentage);

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void CalculateExpectedWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreZero_ShouldSetExpectedWinsAndExpectedLossesToZero()
        {
            // Arrange
            var teamSeason = new TeamSeason
            {
                PointsFor = 0,
                PointsAgainst = 0
            };

            // Act
            teamSeason.CalculateExpectedWinsAndLosses();

            // Assert
            teamSeason.ExpectedWins.ShouldBe(0);
            teamSeason.ExpectedLosses.ShouldBe(0);
        }

        [Fact]
        public void CalculateExpectedWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreNotZero_ShouldSetExpectedWinsAndExpectedLossesToNotZero()
        {
            // Arrange
            var games = 2;
            var pointsFor = 1;
            var pointsAgainst = 1;

            var teamSeason = new TeamSeason
            {
                Games = games,
                PointsFor = pointsFor,
                PointsAgainst = pointsAgainst
            };

            // Act
            teamSeason.CalculateExpectedWinsAndLosses();

            // Assert
            var a = Math.Pow(pointsFor, _exponent);
            var b = Math.Pow(pointsFor, _exponent) + Math.Pow(pointsAgainst, _exponent);
            decimal expPct = (decimal)(a / b);
            teamSeason.ExpectedWins.ShouldBe(expPct * games);
            teamSeason.ExpectedLosses.ShouldBe((1m - expPct) * games);
        }

        [Fact]
        public void UpdateRankings_WhenTeamGamesIsZero_ShouldSetAllRankingsToNull()
        {
            // Arrange
            var teamSeason = new TeamSeason
            {
                Games = 0
            };

            // Act
            decimal teamSeasonScheduleAveragePointsFor = 0;
            decimal teamSeasonScheduleAveragePointsAgainst = 0;
            decimal leagueSeasonAveragePoints = 0;
            teamSeason.UpdateRankings(
                teamSeasonScheduleAveragePointsFor,
                teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints
                );

            // Assert
            teamSeason.OffensiveAverage.ShouldBeNull();
            teamSeason.OffensiveFactor.ShouldBeNull();
            teamSeason.OffensiveIndex.ShouldBeNull();

            teamSeason.DefensiveAverage.ShouldBeNull();
            teamSeason.DefensiveFactor.ShouldBeNull();
            teamSeason.DefensiveIndex.ShouldBeNull();

            teamSeason.FinalExpectedWinningPercentage.ShouldBeNull();
        }

        [Theory]
        [InlineData(0, 0, 0, 0.00d, 0.00d, 0.00d, null, null, null, null, null, null, null)]
        [InlineData(3, 60, 60, 0.00d, 0.00d, 0.00d, 20.00d, null, null, 20.00d, null, null, null)]
        [InlineData(3, 60, 60, 20.00d, 20.00d, 20.00d, 20.00d, 1.000d, 20.00d, 20.00d, 1.000d, 20.00d, 0.500d)]
        [InlineData(3, 45, 75, 20.00d, 20.00d, 20.00d, 15.00d, 0.750d, 15.00d, 25.00d, 1.250d, 25.00d, 0.2296d)]
        [InlineData(3, 75, 45, 20.00d, 20.00d, 20.00d, 25.00d, 1.250d, 25.00d, 15.00d, 0.750d, 15.00d, 0.7704d)]
        public void UpdateRankings_ShouldUpdateRankingsToCorrectValues(
            int games, int pointsFor, int pointsAgainst,
            double teamSeasonScheduleAveragePointsFor, double teamSeasonScheduleAveragePointsAgainst, double leagueSeasonAveragePoints,
            double? expectedOffensiveAverage, double? expectedOffensiveFactor, double? expectedOffensiveIndex,
            double? expectedDefensiveAverage, double? expectedDefensiveFactor, double? expectedDefensiveIndex,
            double? expectedFinalExpectedWinningPercentage)
        {
            var teamSeason = new TeamSeason
            {
                Games = games,
                PointsFor = pointsFor,
                PointsAgainst = pointsAgainst
            };

            // Act
            teamSeason.UpdateRankings(
                (decimal)teamSeasonScheduleAveragePointsFor,
                (decimal)teamSeasonScheduleAveragePointsAgainst,
                (decimal)leagueSeasonAveragePoints
                );

            // Assert
            ((double?)teamSeason.OffensiveAverage).ShouldBe(expectedOffensiveAverage);
            ((double?)teamSeason.OffensiveFactor).ShouldBe(expectedOffensiveFactor);
            ((double?)teamSeason.OffensiveIndex).ShouldBe(expectedOffensiveIndex);

            ((double?)teamSeason.DefensiveAverage).ShouldBe(expectedDefensiveAverage);
            ((double?)teamSeason.DefensiveFactor).ShouldBe(expectedDefensiveFactor);
            ((double?)teamSeason.DefensiveIndex).ShouldBe(expectedDefensiveIndex);

            if (expectedFinalExpectedWinningPercentage == null)
            {
                teamSeason.FinalExpectedWinningPercentage.ShouldBeNull();
            }
            else
            {
                //((double?)teamSeason.FinalExpectedWinningPercentage).ShouldBeCloseTo(expectedFinalExpectedWinningPercentage.Value);
            }
        }
    }
}
