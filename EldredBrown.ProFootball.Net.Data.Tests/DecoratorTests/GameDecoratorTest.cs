using Shouldly;
using Xunit;
using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Tests.DecoratorTests
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
    }
}
