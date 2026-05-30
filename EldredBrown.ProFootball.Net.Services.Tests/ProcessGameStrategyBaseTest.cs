using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class ProcessGameStrategyBaseTest
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ProcessGameStrategyBase _testStrategy;

        public ProcessGameStrategyBaseTest()
        {
            _teamRepository = A.Fake<ITeamRepository>();
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _testStrategy = new ProcessGameStrategyBase(_teamRepository, _teamSeasonRepository);
        }

        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _testStrategy.ProcessGame(null!));
        }

        [Fact(Skip = "This test requires a concrete implementation of ProcessGameStrategyBase.")]
        public async Task ProcessGame_WhenGameArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Guest" },
                new Team { Id = 2, Name = "Host" },
            };
            A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).ReturnsNextFromSequence(teams.ToArray());

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason> { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored)).Returns(teamSeasons);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            _testStrategy.ProcessGame(game);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(2)).MustHaveHappenedOnceExactly();

            //UpdateGamesForTeamSeasons(guestSeason, hostSeason);
            //UpdateWinsLossesAndTiesForTeamSeasons(guestSeason, hostSeason, game);
            //EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            //_teamSeasonRepository.Update(guestSeason);
            //_teamSeasonRepository.Update(hostSeason);
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Func<Task> func = new Func<Task>(async () => await _testStrategy.ProcessGameAsync(null!));

            // Assert
            await func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Fact(Skip = "This test requires a concrete implementation of ProcessGameStrategyBase.")]
        public async Task ProcessGameAsync_WhenGameArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Guest" },
                new Team { Id = 2, Name = "Host" },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).ReturnsNextFromSequence(teams.ToArray());

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason> { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            // Act
            var game = A.Fake<Game>();
            game.GuestName = "Guest";
            game.HostName = "Host";
            _testStrategy.ProcessGame(game);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(2)).MustHaveHappenedOnceExactly();

            //UpdateGamesForTeamSeasons(guestSeason, hostSeason);
            //UpdateWinsLossesAndTiesForTeamSeasons(guestSeason, hostSeason, game);
            //EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            //_teamSeasonRepository.Update(guestSeason);
            //_teamSeasonRepository.Update(hostSeason);
        }
    }
}
