using System;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class ProcessGameStrategyBaseTest
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ProcessGameStrategyBase _testStrategy;

        public ProcessGameStrategyBaseTest()
        {
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _testStrategy = new ProcessGameStrategyBase(_teamSeasonRepository);
        }

        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameDecorator? gameDecorator = null;

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _testStrategy.ProcessGame(null!));
        }

        [Fact(Skip = "This test requires a concrete implementation of ProcessGameStrategyBase.")]
        public async Task ProcessGame_WhenGameDecoratorArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var game = A.Fake<Game>();
            game.GuestName = "Guest";
            game.HostName = "Host";

            // Act
            _testStrategy.ProcessGame(game);

            // Assert
            var seasonYear = game.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(game.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(game.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameDecorator? gameDecorator = null;

            // Act
            Func<Task> func = new Func<Task>(async () => await _testStrategy.ProcessGameAsync(gameDecorator!));

            // Assert
            await func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var game = A.Fake<Game>();
            game.GuestName = "Guest";
            game.HostName = "Host";

            // Act
            try
            {
                await _testStrategy.ProcessGameAsync(game);
            }
            catch (NotImplementedException)
            {
                // This test case calls a base class method that is implemented only in subclasses, thereby throwing a
                // NotImplementedException. The exception has no impact on what I expect to happen here, so it can be
                // ignored.
            }

            // Assert
            var seasonYear = game.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(game.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(game.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
        }
    }
}
