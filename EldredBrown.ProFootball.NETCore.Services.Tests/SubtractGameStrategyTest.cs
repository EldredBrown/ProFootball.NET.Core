using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class SubtractGameStrategyTest
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly SubtractGameStrategy _strategy;

        public SubtractGameStrategyTest()
        {
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _strategy = new SubtractGameStrategy(_teamSeasonRepository);
        }

        [Fact]
        public async Task ProcessGame_WhenGameIsATie_ShouldUpdateTiesForTeamSeasons()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";
            gameDecorator.WinnerName = "Winner";
            gameDecorator.LoserName = "Loser";
            gameDecorator.SeasonYear = 1920;
            A.CallTo(() => gameDecorator.IsTie()).Returns(true);

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns<TeamSeason?>(null);

            // Act
            await _strategy.ProcessGameAsync(gameDecorator);

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.WinnerName, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.LoserName, seasonYear))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task ProcessGame_WhenGameIsNotATie_ShouldUpdateWinsAndLossesForTeamSeasons()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";
            gameDecorator.WinnerName = "Winner";
            gameDecorator.LoserName = "Loser";
            gameDecorator.SeasonYear = 1920;
            A.CallTo(() => gameDecorator.IsTie()).Returns(false);

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns<TeamSeason?>(null);

            // Act
            await _strategy.ProcessGameAsync(gameDecorator);

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.GuestName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.HostName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.WinnerName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.LoserName, seasonYear))
                .MustHaveHappened();
        }
    }
}
