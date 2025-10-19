using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class ProcessGameStrategyFactoryTest
    {
        private readonly IProcessGameStrategyFactory _factory;

        public ProcessGameStrategyFactoryTest()
        {
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _factory = new ProcessGameStrategyFactory(teamSeasonRepository);
        }

        [Fact]
        public void CreateStrategy_WhenDirectionIsUp_ShouldCreateAddGameStrategy()
        {
            // Act
            var strategy = _factory.CreateStrategy(Direction.Up);

            // Assert
            strategy.ShouldBeOfType<AddGameStrategy>();
        }

        [Fact]
        public void CreateStrategy_WhenDirectionIsDown_ShouldCreateSubtractGameStrategy()
        {
            // Act
            var strategy = _factory.CreateStrategy(Direction.Down);

            // Assert
            strategy.ShouldBeOfType<SubtractGameStrategy>();
        }

        [Fact]
        public void CreateStrategy_WhenDirectionIsNotUpNorDown_ShouldCreateNullGameStrategy()
        {
            // Act
            var strategy = _factory.CreateStrategy((Direction)3);

            // Assert
            strategy.ShouldBeOfType<NullGameStrategy>();
        }
    }
}
