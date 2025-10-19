using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Tests
{
    public class GameDecoratorTest
    {
        private readonly GameDecorator _decorator;

        public GameDecoratorTest()
        {
            var game = new Game();
            _decorator = new GameDecorator(game);
        }

        [Fact]
        public void DecideWinnerAndLoser_WhenGuestScoreGreaterThanHostScore_ShouldDeclareGuestTheWinner()
        {
            // Arrange
            _decorator.GuestName = "Guest";
            _decorator.GuestScore = 1;
            _decorator.HostName = "Host";
            _decorator.HostScore = 0;

            // Act
            _decorator.DecideWinnerAndLoser();

            // Assert
            _decorator.WinnerName.ShouldBe(_decorator.GuestName);
            _decorator.WinnerScore.ShouldBe(_decorator.GuestScore);
            _decorator.LoserName.ShouldBe(_decorator.HostName);
            _decorator.LoserScore.ShouldBe(_decorator.HostScore);
        }

        [Fact]
        public void DecideWinnerAndLoser_WhenHostScoreGreaterThanGuestScore_ShouldDeclareHostTheWinner()
        {
            // Arrange
            _decorator.GuestName = "Guest";
            _decorator.GuestScore = 0;
            _decorator.HostName = "Host";
            _decorator.HostScore = 1;

            // Act
            _decorator.DecideWinnerAndLoser();

            // Assert
            _decorator.WinnerName.ShouldBe(_decorator.HostName);
            _decorator.WinnerScore.ShouldBe(_decorator.HostScore);
            _decorator.LoserName.ShouldBe(_decorator.GuestName);
            _decorator.LoserScore.ShouldBe(_decorator.GuestScore);
        }

        [Fact]
        public void DecideWinnerAndLoser_WhenGuestScoreEqualsHostScore_ShouldDeclareNeitherGuestNorHostTheWinner()
        {
            // Arrange
            _decorator.GuestName = "Guest";
            _decorator.GuestScore = 0;
            _decorator.HostName = "Host";
            _decorator.HostScore = 0;

            // Act
            _decorator.DecideWinnerAndLoser();

            // Assert
            _decorator.WinnerName.ShouldBeNull();
            _decorator.LoserName.ShouldBeNull();
        }

        [Fact]
        public void Edit_ShouldUpdateGameDataWithDataFromSource()
        {
            var srcGame = new Game
            {
                Week = 18,
                GuestName = "Cincinnati Bengals",
                GuestScore = 20,
                HostName = "Los Angeles Rams",
                HostScore = 23,
                WinnerName = "Los Angeles Rams",
                WinnerScore = 23,
                LoserName = "Cincinnati Bengals",
                LoserScore = 20,
                IsPlayoff = true,
                Notes = "Super Bowl LVI"
            };
            var srcGameDecorator = new GameDecorator(srcGame);

            _decorator.Edit(srcGameDecorator);

            _decorator.Week.ShouldBe(srcGameDecorator.Week);
            _decorator.GuestName.ShouldBe(srcGameDecorator.GuestName);
            _decorator.GuestScore.ShouldBe(srcGameDecorator.GuestScore);
            _decorator.HostName.ShouldBe(srcGameDecorator.HostName);
            _decorator.HostScore.ShouldBe(srcGameDecorator.HostScore);
            _decorator.WinnerName.ShouldBe(srcGameDecorator.WinnerName);
            _decorator.WinnerScore.ShouldBe(srcGameDecorator.WinnerScore);
            _decorator.LoserName.ShouldBe(srcGameDecorator.LoserName);
            _decorator.LoserScore.ShouldBe(srcGameDecorator.LoserScore);
            _decorator.IsPlayoff.ShouldBe(srcGameDecorator.IsPlayoff);
            _decorator.Notes.ShouldBe(srcGameDecorator.Notes);
        }

        [Fact]
        public void IsTie_WhenGuestScoreIsGreaterThanHostScore_ShouldReturnFalse()
        {
            _decorator.GuestScore = 1;
            _decorator.HostScore = 0;

            var result = _decorator.IsTie();

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenHostScoreIsGreaterThanGuestScore_ShouldReturnFalse()
        {
            _decorator.GuestScore = 0;
            _decorator.HostScore = 1;

            var result = _decorator.IsTie();

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenGuestScoreEqualsHostScore_ShouldReturnTrue()
        {
            _decorator.GuestScore = 0;
            _decorator.HostScore = 0;

            var result = _decorator.IsTie();

            result.ShouldBeTrue();
        }
    }
}
