using System;
using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;

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

        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";

            // Act
            _testStrategy.ProcessGame(gameDecorator);

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameDecoratorArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameDecorator? gameDecorator = null;

            // Act
            Func<Task> func = new Func<Task>(async () => await _testStrategy.ProcessGameAsync(gameDecorator!));

            // Assert
            await func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameDecoratorArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";

            // Act
            try
            {
                await _testStrategy.ProcessGameAsync(gameDecorator);
            }
            catch (NotImplementedException)
            {
                // This test case calls a base class method that is implemented only in subclasses, thereby throwing a
                // NotImplementedException. The exception has no impact on what I expect to happen here, so it can be
                // ignored.
            }

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
        }
    }
}
