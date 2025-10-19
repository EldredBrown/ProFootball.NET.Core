using System;
using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class ProcessGameStrategyBaseTest
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ProcessGameStrategyBase _strategy;

        public ProcessGameStrategyBaseTest()
        {
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _strategy = new ProcessGameStrategyBase(_teamSeasonRepository);
        }

        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameDecorator? gameDecorator = null;

            // Act
            Func<Task> func = new Func<Task>(async () => await _strategy.ProcessGameAsync(gameDecorator!));

            // Assert
            await func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";

            // Act
            try
            {
                await _strategy.ProcessGameAsync(gameDecorator);
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
