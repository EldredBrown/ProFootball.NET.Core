using Shouldly;
using Xunit;
using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.DecoratorTests
{
    public class ConferenceRepositoryTest
    {
        private readonly GameDecorator _decorator;

        public ConferenceRepositoryTest()
        {
            var game = new Game();
            _decorator = new GameDecorator(game);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesFromGame()
        {
            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Test"
            };

            var decorator = new GameDecorator(game);
            decorator.Id.ShouldBe(game.Id);
            decorator.SeasonYear.ShouldBe(game.SeasonYear);
            decorator.Week.ShouldBe(game.Week);
            decorator.GuestName.ShouldBe(game.GuestName);
            decorator.GuestScore.ShouldBe(game.GuestScore);
            decorator.HostName.ShouldBe(game.HostName);
            decorator.HostScore.ShouldBe(game.HostScore);
            decorator.IsPlayoff.ShouldBe(game.IsPlayoff);
            decorator.Notes.ShouldBe(game.Notes);
        }

        [Fact]
        public void IsTie_WhenGuestScoreIsGreaterThanHostScore_ShouldReturnFalse()
        {
            _decorator.GuestScore = 1;
            _decorator.HostScore = 0;

            var result = _decorator.IsTie;

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenHostScoreIsGreaterThanGuestScore_ShouldReturnFalse()
        {
            _decorator.GuestScore = 0;
            _decorator.HostScore = 1;

            var result = _decorator.IsTie;

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsTie_WhenGuestScoreEqualsHostScore_ShouldReturnTrue()
        {
            _decorator.GuestScore = 0;
            _decorator.HostScore = 0;

            var result = _decorator.IsTie;

            result.ShouldBeTrue();
        }

        [Fact]
        public void WinnerLoserProperties_WhenGameIsTie_ShouldReturnNull()
        {
            _decorator.GuestScore = 3;
            _decorator.HostScore = 3;

            _decorator.WinnerName.ShouldBeNull();
            _decorator.WinnerScore.ShouldBeNull();
            _decorator.LoserName.ShouldBeNull();
            _decorator.LoserScore.ShouldBeNull();
        }

        [Fact]
        public void WinnerLoserProperties_WhenGuestScoreGreaterThanHostScore_ShouldReturnCorrectValues()
        {
            _decorator.GuestScore = 3;
            _decorator.HostScore = 2;

            _decorator.WinnerName.ShouldBe("Guest");
            _decorator.WinnerScore.ShouldBe(3);
            _decorator.LoserName.ShouldBe("Host");
            _decorator.LoserScore.ShouldBe(2);
        }

        [Fact]
        public void WinnerLoserProperties_WhenHostScoreGreaterThanGuestScore_ShouldReturnCorrectValues()
        {
            _decorator.GuestScore = 2;
            _decorator.HostScore = 3;

            _decorator.WinnerName.ShouldBe("Host");
            _decorator.WinnerScore.ShouldBe(3);
            _decorator.LoserName.ShouldBe("Guest");
            _decorator.LoserScore.ShouldBe(2);
        }

        [Fact]
        public void Edit_ShouldUpdateGameDataWithDataFromSource()
        {
            var srcGame = new Game
            {
                Week = 1,
                GuestName = "Guest",
                GuestScore = 3,
                HostName = "Host",
                HostScore = 3,
                IsPlayoff = true,
                Notes = "Notes"
            };
            var srcGameDecorator = new GameDecorator(srcGame);

            _decorator.Edit(srcGameDecorator);

            _decorator.Week.ShouldBe(srcGameDecorator.Week);
            _decorator.GuestName.ShouldBe(srcGameDecorator.GuestName);
            _decorator.GuestScore.ShouldBe(srcGameDecorator.GuestScore);
            _decorator.HostName.ShouldBe(srcGameDecorator.HostName);
            _decorator.HostScore.ShouldBe(srcGameDecorator.HostScore);
            _decorator.IsPlayoff.ShouldBe(srcGameDecorator.IsPlayoff);
            _decorator.Notes.ShouldBe(srcGameDecorator.Notes);
        }
    }
}
