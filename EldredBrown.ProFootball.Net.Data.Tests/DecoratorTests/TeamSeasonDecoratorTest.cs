using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.DecoratorTests
{
    public class TeamSeasonDecoratorTest
    {
        private const double _exponent = 2.37;

        private readonly ITeamSeasonDecorator _decorator;

        public TeamSeasonDecoratorTest()
        {
            var teamSeason = new TeamSeason();
            _decorator = new TeamSeasonDecorator(teamSeason);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesFromTeamSeason()
        {
            var teamSeason = new TeamSeason
            {
                Games = 2,
                Wins = 1,
                Losses = 1,
                Ties = 0,
                PointsFor = 40,
                PointsAgainst = 40
            };

            var decorator = new TeamSeasonDecorator(teamSeason);
            decorator.Games.ShouldBe(teamSeason.Games);
            decorator.Wins.ShouldBe(teamSeason.Wins);
            decorator.Losses.ShouldBe(teamSeason.Losses);
            decorator.Ties.ShouldBe(teamSeason.Ties);
            decorator.PointsFor.ShouldBe(teamSeason.PointsFor);
            decorator.PointsAgainst.ShouldBe(teamSeason.PointsAgainst);
        }

        [Fact]
        public void WinningPercentage_WhenTeamHasNoGames_ShouldReturnNull()
        {
            _decorator.Games = 0;
            var result = _decorator.WinningPercentage;
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(0, 0, 0, 0, null)]
        [InlineData(2, 2, 0, 0, 1)]
        [InlineData(2, 1, 1, 0, 0.5)]
        [InlineData(2, 0, 2, 0, 0)]
        [InlineData(2, 1, 0, 1, 0.75)]
        [InlineData(2, 0, 1, 1, 0.25)]
        [InlineData(2, 0, 0, 2, 0.5)]
        public void WinningPercentage_ShouldReturnCorrectValue(int games, int wins, int losses, int ties, decimal? expected)
        {
            _decorator.Games = games;
            _decorator.Wins = wins;
            _decorator.Losses = losses;
            _decorator.Ties = ties;
            var result = _decorator.WinningPercentage;
            result.ShouldBe(expected);
        }

        [Fact]
        public void CalculateExpectedWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreZero_ShouldSetExpectedWinsAndExpectedLossesToZero()
        {
            // Arrange
            _decorator.PointsFor = 0;
            _decorator.PointsAgainst = 0;

            // Act
            _decorator.CalculateExpectedWinsAndLosses();

            // Assert
            _decorator.ExpectedWins.ShouldBe(0);
            _decorator.ExpectedLosses.ShouldBe(0);
        }

        [Fact]
        public void CalculateExpectedWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreNotZero_ShouldSetExpectedWinsAndExpectedLossesToNotZero()
        {
            // Arrange
            var games = 2;
            var pointsFor = 1;
            var pointsAgainst = 1;

            _decorator.Games = games;
            _decorator.PointsFor = pointsFor;
            _decorator.PointsAgainst = pointsAgainst;

            // Act
            _decorator.CalculateExpectedWinsAndLosses();

            // Assert
            var a = Math.Pow(pointsFor, _exponent);
            var b = Math.Pow(pointsFor, _exponent) + Math.Pow(pointsAgainst, _exponent);
            decimal expPct = (decimal)(a / b)  ;
            _decorator.ExpectedWins.ShouldBe(expPct * games);
            _decorator.ExpectedLosses.ShouldBe((1m - expPct) * games);
        }

        [Fact]
        public void UpdateRankings_WhenTeamGamesIsZero_ShouldSetAllRankingsToNull()
        {
            // Arrange
            _decorator.Games = 0;

            // Act
            decimal teamSeasonScheduleAveragePointsFor = 0;
            decimal teamSeasonScheduleAveragePointsAgainst = 0;
            decimal leagueSeasonAveragePoints = 0;
            _decorator.UpdateRankings(teamSeasonScheduleAveragePointsFor, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBeNull();
            _decorator.OffensiveFactor.ShouldBeNull();
            _decorator.OffensiveIndex.ShouldBeNull();

            _decorator.DefensiveAverage.ShouldBeNull();
            _decorator.DefensiveFactor.ShouldBeNull();
            _decorator.DefensiveIndex.ShouldBeNull();

            _decorator.FinalExpectedWinningPercentage.ShouldBeNull();
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, null, null, null, null, null, null, null)]
        [InlineData(3, 60, 60, 0, 0, 0, 20, null, null, 20, null, null, null)]
        [InlineData(3, 60, 60, 20, 20, 20, 20, 1, 20, 20, 1, 20, 0.5)]
        [InlineData(3, 45, 75, 20, 20, 20, 15, 0.75, 15, 25, 1.25, 25, 0.2296)]
        [InlineData(3, 75, 45, 20, 20, 20, 25, 1.25, 25, 15, 0.75, 15, 0.7704)]
        public void UpdateRankings_ShouldUpdateRankingsToCorrectValues(
            int games, int pointsFor, int pointsAgainst,
            decimal teamSeasonScheduleAveragePointsFor, decimal teamSeasonScheduleAveragePointsAgainst, decimal leagueSeasonAveragePoints,
            decimal? expectedOffensiveAverage, decimal? expectedOffensiveFactor, decimal? expectedOffensiveIndex,
            decimal? expectedDefensiveAverage, decimal? expectedDefensiveFactor, decimal? expectedDefensiveIndex,
            decimal? expectedFinalExpectedWinningPercentage)
        {
            _decorator.Games = games;
            _decorator.PointsFor = pointsFor;
            _decorator.PointsAgainst = pointsAgainst;

            // Act
            _decorator.UpdateRankings(
                teamSeasonScheduleAveragePointsFor,
                teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBe(expectedOffensiveAverage);
            _decorator.OffensiveFactor.ShouldBe(expectedOffensiveFactor);
            _decorator.OffensiveIndex.ShouldBe(expectedOffensiveIndex);

            _decorator.DefensiveAverage.ShouldBe(expectedDefensiveAverage);
            _decorator.DefensiveFactor.ShouldBe(expectedDefensiveFactor);
            _decorator.DefensiveIndex.ShouldBe(expectedDefensiveIndex);

            if (expectedFinalExpectedWinningPercentage == null)
            {
                _decorator.FinalExpectedWinningPercentage.ShouldBeNull();
            }
            else
            {
                //_decorator.FinalExpectedWinningPercentage.Value.ShouldBeCloseTo(expectedFinalExpectedWinningPercentage.Value);
            }
        }
    }
}
