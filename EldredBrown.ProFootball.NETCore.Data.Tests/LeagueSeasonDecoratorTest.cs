using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using Shouldly;
using Xunit;

namespace EldredBrown.ProFootball.NETCore.Data.Tests
{
    public class LeagueSeasonDecoratorTest
    {
        private readonly LeagueSeasonDecorator _testDecorator;

        public LeagueSeasonDecoratorTest()
        {
            var leagueSeason = new LeagueSeason();
            _testDecorator = new LeagueSeasonDecorator(leagueSeason);
        }

        [Fact]
        public void UpdateGamesAndPoints_WhenGamesEqualZero_ShouldUpdateOnlyGamesAndPoints()
        {
            // Arrange
            _testDecorator.TotalGames = 1;
            _testDecorator.TotalPoints = 1;

            // Act
            _testDecorator.UpdateGamesAndPoints(totalGames: 0, totalPoints: 0);

            // Assert
            _testDecorator.TotalGames.ShouldBe(0);
            _testDecorator.TotalPoints.ShouldBe(0);
            _testDecorator.AveragePoints.ShouldBeNull();
        }

        [Fact]
        public void UpdateGamesAndPoints_WhenGamesNotEqualZero_ShouldUpdateGamesAndPointsAndAverage()
        {
            // Arrange
            _testDecorator.TotalGames = 0;
            _testDecorator.TotalPoints = 0;

            // Act
            _testDecorator.UpdateGamesAndPoints(totalGames: 1, totalPoints: 1);

            // Assert
            _testDecorator.TotalGames.ShouldBe(1);
            _testDecorator.TotalPoints.ShouldBe(1);
            _testDecorator.AveragePoints.ShouldBe(1d);
        }
    }
}
