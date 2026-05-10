using Shouldly;
using Xunit;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class GamePredictorServiceTest
    {
        [Fact]
        public void PredictGameScore_ShouldReturnCorrectlyCalculatedPredictedGameScores()
        {
            // Arrange
            var testObject = new GamePredictorService();

            var guestSeason = new TeamSeason
            {
                OffensiveAverage = 7.00m,
                OffensiveFactor = 0.500m,
                DefensiveAverage = 14.00m,
                DefensiveFactor = 1.500m
            };
            var hostSeason = new TeamSeason
            {
                OffensiveAverage = 28.00m,
                OffensiveFactor = 2.000m,
                DefensiveAverage = 21.00m,
                DefensiveFactor = 1.000m
            };

            // Act
            var (predictedGuestScore, predictedHostScore) =
                testObject.PredictGameScore(guestSeason, hostSeason);

            // Assert
            predictedGuestScore.ShouldBe((guestSeason.OffensiveFactor * hostSeason.DefensiveAverage +
                hostSeason.DefensiveFactor * guestSeason.OffensiveAverage) / 2m);
            predictedHostScore.ShouldBe((hostSeason.OffensiveFactor * guestSeason.DefensiveAverage +
                guestSeason.DefensiveFactor * hostSeason.OffensiveAverage) / 2m);
        }
    }
}
