using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.ModelTests
{
    public class TeamSeasonTest
    {
        [Theory]
        [InlineData(2, 1, 0, 1, 0.750d)]
        [InlineData(2, 0, 1, 1, 0.250d)]
        [InlineData(2, 0, 0, 2, 0.500d)]
        [InlineData(2, 2, 0, 0, 1.000d)]
        [InlineData(2, 1, 1, 0, 0.500d)]
        [InlineData(2, 0, 2, 0, 0.000d)]
        [InlineData(0, 0, 0, 0, null)]
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
            double? result = (double?)teamSeason.WinningPercentage;

            // Assert
            result.ShouldBe(expected);
        }
    }
}
