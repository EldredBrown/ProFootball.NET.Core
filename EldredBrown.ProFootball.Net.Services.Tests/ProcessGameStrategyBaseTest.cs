using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class ProcessGameStrategyBaseTest
    {
        [Fact]
        public async Task ProcessGame_WhenGameDecoratorArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ProcessGameStrategyBase testStrategy = SetUp();

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => testStrategy.ProcessGame(null!));
        }

        [Fact(Skip = "This test requires a concrete implementation of ProcessGameStrategyBase.")]
        public async Task ProcessGame_WhenGameArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Guest" },
                new() { Id = 2, Name = "Host" },
            };
            ProcessGameStrategyBase testStrategy = SetUp(teams: teams);

            // Act
            var game = new Game
            {
                SeasonYear = 1920,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            testStrategy.ProcessGame(game);

            // Assert
            A.CallTo(() => testStrategy._teamSeasonRepository.GetTeamSeasonsBySeason(game.SeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testStrategy._teamRepository.GetTeam(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testStrategy._teamRepository.GetTeam(2)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ProcessGameStrategyBase testStrategy = SetUp();

            // Act
            Func<Task> func = new Func<Task>(async () => await testStrategy.ProcessGameAsync(null!));

            // Assert
            await func.ShouldThrowAsync<ArgumentNullException>();
        }

        [Fact(Skip = "This test requires a concrete implementation of ProcessGameStrategyBase.")]
        public async Task ProcessGameAsync_WhenGameArgIsNotNull_ShouldProcessGame()
        {
            // Arrange
            var asyncTeams = new List<Team>
            {
                new() { Id = 1, Name = "Guest" },
                new() { Id = 2, Name = "Host" },
            };
            ProcessGameStrategyBase testStrategy = SetUp(asyncTeams: asyncTeams);

            // Act
            var game = A.Fake<Game>();
            game.GuestName = "Guest";
            game.HostName = "Host";
            testStrategy.ProcessGame(game);

            // Assert
            A.CallTo(() => testStrategy._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testStrategy._teamRepository.GetTeam(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testStrategy._teamRepository.GetTeam(2)).MustHaveHappenedOnceExactly();
        }

        private static ProcessGameStrategyBase SetUp(List<Team>? teams = null, List<Team>? asyncTeams = null)
        {
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            if (teams is not null)
            {
                A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).ReturnsNextFromSequence([.. teams]);
            }
            else if (asyncTeams is not null)
            {
                A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).ReturnsNextFromSequence([.. asyncTeams]);
            }

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored)).Returns(teamSeasons);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            return new ProcessGameStrategyBase(fakeTeamRepository, fakeTeamSeasonRepository);
        }
    }
}
