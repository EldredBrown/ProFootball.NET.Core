using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class AddGameStrategyTest
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly AddGameStrategy _strategy;

        public AddGameStrategyTest()
        {
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _strategy = new AddGameStrategy(_teamSeasonRepository);
        }

        [Fact]
        public void ProcessGame_WhenGameIsATie_ShouldUpdateTiesForTeamSeasons()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";
            gameDecorator.WinnerName = "Winner";
            gameDecorator.LoserName = "Loser";
            gameDecorator.SeasonYear = 1920;
            A.CallTo(() => gameDecorator.IsTie()).Returns(true);

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
                .Returns<TeamSeason?>(null);

            // Act
            _strategy.ProcessGame(gameDecorator);

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.WinnerName, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.LoserName, seasonYear))
                .MustNotHaveHappened();
        }

        [Fact]
        public void ProcessGame_WhenGameIsNotATie_ShouldUpdateWinsAndLossesForTeamSeasons()
        {
            // Arrange
            var gameDecorator = A.Fake<IGameDecorator>();
            gameDecorator.GuestName = "Guest";
            gameDecorator.HostName = "Host";
            gameDecorator.WinnerName = "Winner";
            gameDecorator.LoserName = "Loser";
            gameDecorator.SeasonYear = 1920;
            A.CallTo(() => gameDecorator.IsTie()).Returns(false);

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
                .Returns<TeamSeason?>(null);

            // Act
            _strategy.ProcessGame(gameDecorator);

            // Assert
            var seasonYear = gameDecorator.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.GuestName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.HostName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.WinnerName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.LoserName, seasonYear))
                .MustHaveHappened();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameIsATie_ShouldUpdateTiesForTeamSeasons()
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
        public async Task ProcessGameAsync_WhenGameIsNotATie_ShouldUpdateWinsAndLossesForTeamSeasons()
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
