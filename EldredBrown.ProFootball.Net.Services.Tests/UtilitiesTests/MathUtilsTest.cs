using EldredBrown.ProFootball.Net.Services.Utilities;
using Shouldly;
using System;
using Xunit;

namespace EldredBrown.ProFootball.Net.Services.Tests.UtilitiesTests
{
    public class MathUtilsTest
    {
        private const double _exponent = 2.37;

        [Fact]
        public void CalculateExpectedWinningPercentage_WhenPointsForIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            decimal pointsFor = -1;
            decimal pointsAgainst = 0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(
                () => MathUtils.CalculateExpectedWinningPercentage(pointsFor, pointsAgainst));
        }

        [Fact]
        public void CalculateExpectedWinningPercentage_WhenPointsAgainstIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            decimal pointsFor = 0;
            decimal pointsAgainst = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(
                () => MathUtils.CalculateExpectedWinningPercentage(pointsFor, pointsAgainst));
        }

        [Fact]
        public void CalculateExpectedWinningPercentage_WhenPointsAgainstIsZero_ShouldReturnNull()
        {
            // Arrange
            decimal pointsFor = 0;
            decimal pointsAgainst = 0;

            // Act
            var result = MathUtils.CalculateExpectedWinningPercentage(pointsFor, pointsAgainst);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(2, 1)]
        [InlineData(1, 2)]
        [InlineData(0, 1)]
        public void CalculateExpectedWinningPercentage_WhenPointsAgainstIsNotZero_ShouldReturnCorrectValue(
            int pointsFor, int pointsAgainst)
        {
            // Act
            var result = MathUtils.CalculateExpectedWinningPercentage(pointsFor, pointsAgainst);

            // Assert
            var o = Math.Pow((double)pointsFor, _exponent);
            var d = Math.Pow((double)pointsAgainst, _exponent);
            var expected = (decimal)o / (decimal)(o + d);
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(2, 1)]
        [InlineData(1, 2)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public void Divide_ShouldReturnCorrectValue(decimal numerator, decimal denominator)
        {
            // Act
            var result = MathUtils.Divide(numerator, denominator);

            // Assert
            if(denominator == 0)
            {
                result.ShouldBeNull();
            }
            else
            {
                var expected = numerator / denominator;
                result.ShouldBe(expected);
            }
        }
    }
}
