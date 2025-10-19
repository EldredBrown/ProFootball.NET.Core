using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Tests
{
    public class TeamSeasonDecoratorTest
    {
        private const double _exponent = 2.37;

        private readonly TeamSeasonDecorator _decorator;

        public TeamSeasonDecoratorTest()
        {
            var teamSeason = new TeamSeason();
            _decorator = new TeamSeasonDecorator(teamSeason);
        }

        [Fact]
        public void CalculatePythagoreanWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreZero_ShouldSetPythagoreanWinsAndPythagoreanLossesToZero()
        {
            // Arrange
            _decorator.PointsFor = 0;
            _decorator.PointsAgainst = 0;

            // Act
            _decorator.CalculatePythagoreanWinsAndLosses();

            // Assert
            _decorator.PythagoreanWins.ShouldBe(0);
            _decorator.PythagoreanLosses.ShouldBe(0);
        }

        [Fact]
        public void CalculatePythagoreanWinsAndLosses_WhenTeamSeasonPointsForAndPointsAgainstAreNotZero_ShouldSetPythagoreanWinsAndPythagoreanLossesToNotZero()
        {
            // Arrange
            var games = 2;
            var pointsFor = 40;
            var pointsAgainst = 40;

            _decorator.Games = games;
            _decorator.PointsFor = pointsFor;
            _decorator.PointsAgainst = pointsAgainst;

            // Act
            _decorator.CalculatePythagoreanWinsAndLosses();

            // Assert
            var a = Math.Pow(pointsFor, _exponent);
            var b = Math.Pow(pointsFor, _exponent) + Math.Pow(pointsAgainst, _exponent);
            double pythPct = a / b;
            _decorator.PythagoreanWins.ShouldBe(pythPct * games);
            _decorator.PythagoreanLosses.ShouldBe((1d - pythPct) * games);
        }

        [Fact]
        public void CalculateWinningPercentage_WhenTeamHasNoGames_ShouldSetWinningPercentageToNull()
        {
            // Arrange
            _decorator.Games = 0;

            // Act
            _decorator.CalculateWinningPercentage();

            // Assert
            _decorator.WinningPercentage.ShouldBeNull();
        }

        [Fact]
        public void CalculateWinningPercentage_WhenTeamHasWinningRecord()
        {
            // Arrange
            double games = 2;
            double wins = 1;
            double ties = 1;

            _decorator.Games = (int)games;
            _decorator.Wins = (int)wins;
            _decorator.Ties = (int)ties;

            // Act
            _decorator.CalculateWinningPercentage();

            // Assert
            var pct = ((2 * wins) + ties) / (2 * games);
            _decorator.WinningPercentage.ShouldBe(pct);
        }

        [Fact]
        public void CalculateWinningPercentage_WhenTeamHasLosingRecord()
        {
            // Arrange
            double games = 2;
            double wins = 0;
            double ties = 1;

            _decorator.Games = (int)games;
            _decorator.Wins = (int)wins;
            _decorator.Ties = (int)ties;

            // Act
            _decorator.CalculateWinningPercentage();

            // Assert
            var pct = ((2 * wins) + ties) / (2 * games);
            _decorator.WinningPercentage.ShouldBe(pct);
        }

        [Fact]
        public void CalculateWinningPercentage_WhenTeamHas500RecordWithoutTies()
        {
            // Arrange
            double games = 2;
            double wins = 1;
            double ties = 0;

            _decorator.Games = (int)games;
            _decorator.Wins = (int)wins;
            _decorator.Ties = (int)ties;

            // Act
            _decorator.CalculateWinningPercentage();

            // Assert
            var pct = ((2 * wins) + ties) / (2 * games);
            _decorator.WinningPercentage.ShouldBe(pct);
        }

        [Fact]
        public void CalculateWinningPercentage_WhenTeamHas500RecordWithTies()
        {
            // Arrange
            double games = 2;
            double wins = 0;
            double ties = 2;

            _decorator.Games = (int)games;
            _decorator.Wins = (int)wins;
            _decorator.Ties = (int)ties;

            // Act
            _decorator.CalculateWinningPercentage();

            // Assert
            var pct = ((2 * wins) + ties) / (2 * games);
            _decorator.WinningPercentage.ShouldBe(pct);
        }

        [Fact]
        public void UpdateRankings_WhenTeamGamesIsZero_ShouldSetAllRankingsToNull()
        {
            // Arrange
            _decorator.Games = 0;

            // Act
            double teamSeasonScheduleAveragePointsFor = 0;
            double teamSeasonScheduleAveragePointsAgainst = 0;
            double leagueSeasonAveragePoints = 0;
            _decorator.UpdateRankings(teamSeasonScheduleAveragePointsFor, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBeNull();
            _decorator.OffensiveFactor.ShouldBeNull();
            _decorator.OffensiveIndex.ShouldBeNull();

            _decorator.DefensiveAverage.ShouldBeNull();
            _decorator.DefensiveFactor.ShouldBeNull();
            _decorator.DefensiveIndex.ShouldBeNull();

            _decorator.FinalPythagoreanWinningPercentage.ShouldBeNull();
        }

        [Fact]
        public void UpdateRankings_WhenTeamSeasonScheduleAveragePointsAgainstIsZero_ShouldSetOffensiveFactorToNull()
        {
            // Arrange
            double games = 2;
            double pointsFor = 30;
            double pointsAgainst = 30;

            _decorator.Games = (int)games;
            _decorator.PointsFor = (int)pointsFor;
            _decorator.PointsAgainst = (int)pointsAgainst;

            // Act
            double teamSeasonScheduleAveragePointsFor = 0;
            double teamSeasonScheduleAveragePointsAgainst = 0;
            double leagueSeasonAveragePoints = 0;
            _decorator.UpdateRankings(teamSeasonScheduleAveragePointsFor, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBe(pointsFor / games);
            _decorator.OffensiveFactor.ShouldBeNull();
            _decorator.OffensiveIndex.ShouldBeNull();

            _decorator.DefensiveAverage.ShouldBe(pointsAgainst / games);
            _decorator.DefensiveFactor.ShouldBeNull();
            _decorator.DefensiveIndex.ShouldBeNull();

            _decorator.FinalPythagoreanWinningPercentage.ShouldBeNull();
        }

        [Fact]
        public void UpdateRankings_WhenTeamSeasonScheduleAveragePointsAgainstIsNotZero_ShouldSetOffensiveFactorToNotNull()
        {
            // Arrange
            double games = 2;
            double pointsFor = 30;
            double pointsAgainst = 30;

            _decorator.Games = (int)games;
            _decorator.PointsFor = (int)pointsFor;
            _decorator.PointsAgainst = (int)pointsAgainst;

            // Act
            double teamSeasonScheduleAveragePointsFor = 0;
            double teamSeasonScheduleAveragePointsAgainst = 20;
            double leagueSeasonAveragePoints = 0;
            _decorator.UpdateRankings(teamSeasonScheduleAveragePointsFor, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBe(pointsFor / games);
            _decorator.OffensiveFactor.ShouldBe(_decorator.OffensiveAverage! / teamSeasonScheduleAveragePointsAgainst);
            _decorator.OffensiveIndex.ShouldBe(
                (_decorator.OffensiveAverage! + _decorator.OffensiveFactor! * leagueSeasonAveragePoints) / 2d);

            _decorator.DefensiveAverage.ShouldBe(pointsAgainst / games);
            _decorator.DefensiveFactor.ShouldBeNull();
            _decorator.DefensiveIndex.ShouldBeNull();

            _decorator.FinalPythagoreanWinningPercentage.ShouldBeNull();
        }

        [Fact]
        public void UpdateRankings_WhenTeamSeasonScheduleAveragePointsAreNotZero_ShouldSetAllRankingsToNotNull()
        {
            // Arrange
            double games = 2;
            double pointsFor = 30;
            double pointsAgainst = 30;

            _decorator.Games = (int)games;
            _decorator.PointsFor = (int)pointsFor;
            _decorator.PointsAgainst = (int)pointsAgainst;

            // Act
            double teamSeasonScheduleAveragePointsFor = 20;
            double teamSeasonScheduleAveragePointsAgainst = 20;
            double leagueSeasonAveragePoints = 0;
            _decorator.UpdateRankings(teamSeasonScheduleAveragePointsFor, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            // Assert
            _decorator.OffensiveAverage.ShouldBe(pointsFor / games);
            _decorator.OffensiveFactor.ShouldBe(_decorator.OffensiveAverage! / teamSeasonScheduleAveragePointsAgainst);
            _decorator.OffensiveIndex.ShouldBe(
                (_decorator.OffensiveAverage! + _decorator.OffensiveFactor! * leagueSeasonAveragePoints) / 2d);

            _decorator.DefensiveAverage.ShouldBe(pointsAgainst / games);
            _decorator.DefensiveFactor.ShouldBe(_decorator.DefensiveAverage! / teamSeasonScheduleAveragePointsFor);
            _decorator.DefensiveIndex.ShouldBe(
                (_decorator.DefensiveAverage! + _decorator.DefensiveFactor! * leagueSeasonAveragePoints) / 2d);

            var pfAvg = _decorator.OffensiveIndex!;
            var paAvg = _decorator.DefensiveIndex!;
            var o = Math.Pow(pfAvg.Value, _exponent);
            var d = Math.Pow(paAvg.Value, _exponent);
            _decorator.FinalPythagoreanWinningPercentage.ShouldBe(o / (o + d));
        }
    }
}
