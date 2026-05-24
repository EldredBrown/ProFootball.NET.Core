using System;

using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class GamePredictorServiceTest
    {
        [Fact]
        public void PredictGameScore_WhenNoNullValues_ShouldReturnCorrectlyCalculatedPredictedGameScores()
        {
            // Arrange
            var testService = new GamePredictorService();

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
            var predictedGameScore = testService.PredictGameScore(guestSeason, hostSeason);

            // Assert
            predictedGameScore.GuestScore.ShouldBe((int?)Math.Round((
                guestSeason.OffensiveFactor.Value * hostSeason.DefensiveAverage.Value
                + hostSeason.DefensiveFactor.Value * guestSeason.OffensiveAverage.Value) / 2m, 0));
            predictedGameScore.HostScore.ShouldBe((int?)Math.Round((
                hostSeason.OffensiveFactor.Value * guestSeason.DefensiveAverage.Value
                + guestSeason.DefensiveFactor.Value * hostSeason.OffensiveAverage.Value) / 2m, 0));
        }

        [Fact]
        public void PredictGameScore_WhenNullValues_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var testService = new GamePredictorService();

            var guestSeason = new TeamSeason
            {
                OffensiveAverage = null,
                OffensiveFactor = null,
                DefensiveAverage = null,
                DefensiveFactor = null
            };
            var hostSeason = new TeamSeason
            {
                OffensiveAverage = null,
                OffensiveFactor = null,
                DefensiveAverage = null,
                DefensiveFactor = null
            };

            // Act
            var exception = Record.Exception(() => testService.PredictGameScore(guestSeason, hostSeason));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOperationException>();
        }
    }
}
