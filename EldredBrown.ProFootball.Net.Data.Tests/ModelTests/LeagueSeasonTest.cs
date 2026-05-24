using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.ModelTests
{
    public class LeagueSeasonTest
    {
        private readonly LeagueSeason _testLeagueSeason;

        public LeagueSeasonTest()
        {
            _testLeagueSeason = new LeagueSeason();
        }

        [Fact]
        public void UpdateGamesAndPoints_WhenGamesEqualZero_ShouldUpdateOnlyGamesAndPoints()
        {
            // Arrange
            _testLeagueSeason.TotalGames = 1;
            _testLeagueSeason.TotalPoints = 1;

            // Act
            _testLeagueSeason.UpdateGamesAndPoints(totalGames: 0, totalPoints: 0);

            // Assert
            _testLeagueSeason.TotalGames.ShouldBe(0);
            _testLeagueSeason.TotalPoints.ShouldBe(0);
            _testLeagueSeason.AveragePoints.ShouldBeNull();
        }

        [Fact]
        public void UpdateGamesAndPoints_WhenGamesNotEqualZero_ShouldUpdateGamesAndPointsAndAverage()
        {
            // Arrange
            _testLeagueSeason.TotalGames = 0;
            _testLeagueSeason.TotalPoints = 0;

            // Act
            _testLeagueSeason.UpdateGamesAndPoints(totalGames: 2, totalPoints: 1);

            // Assert
            _testLeagueSeason.TotalGames.ShouldBe(2);
            _testLeagueSeason.TotalPoints.ShouldBe(1);
            _testLeagueSeason.AveragePoints.ShouldBe(0.5m);
        }
    }
}
