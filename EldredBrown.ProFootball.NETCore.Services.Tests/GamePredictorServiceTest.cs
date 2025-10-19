using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
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
                OffensiveAverage = 7.00d,
                OffensiveFactor = 0.500d,
                DefensiveAverage = 14.00d,
                DefensiveFactor = 1.500d
            };
            var hostSeason = new TeamSeason
            {
                OffensiveAverage = 28.00d,
                OffensiveFactor = 2.000d,
                DefensiveAverage = 21.00d,
                DefensiveFactor = 1.000d
            };

            // Act
            var (predictedGuestScore, predictedHostScore) =
                testObject.PredictGameScore(guestSeason, hostSeason);

            // Assert
            predictedGuestScore.ShouldBe((guestSeason.OffensiveFactor * hostSeason.DefensiveAverage +
                hostSeason.DefensiveFactor * guestSeason.OffensiveAverage) / 2d);
            predictedHostScore.ShouldBe((hostSeason.OffensiveFactor * guestSeason.DefensiveAverage +
                guestSeason.DefensiveFactor * hostSeason.OffensiveAverage) / 2d);
        }
    }
}
