using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class AddGameStrategyTest
    {
        private const double _exponent = 2.37;

        [Theory]
        [InlineData(1, 1, 0, 0, 0, 0, 1)]
        [InlineData(2, 1, 1, 0, 0, 1, 0)]
        [InlineData(1, 2, 0, 1, 1, 0, 0)]
        public void ProcessGame_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(int guestScore,
            int hostScore, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason
            {
                TeamId = 1,
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };
            var hostSeason = new TeamSeason
            {
                TeamId = 2,
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored))
                .Returns(new List<TeamSeason> { guestSeason, hostSeason });

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            var hostName = "Host";
            var host = new Team { Name = hostName };
            A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).ReturnsNextFromSequence([guest, guest, host, host]);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = guestName,
                GuestScore = guestScore,
                HostName = hostName,
                HostScore = hostScore
            };
            testStrategy.ProcessGame(game);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(guestSeason.TeamId)).MustHaveHappened();
            A.CallTo(() => fakeTeamRepository.GetTeam(hostSeason.TeamId)).MustHaveHappened();

            guestSeason.Games.ShouldBe(1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(guestScore);
            guestSeason.PointsAgainst.ShouldBe(hostScore);
            var go = (decimal)Math.Pow(guestSeason.PointsFor, _exponent);
            var gd = (decimal)Math.Pow(guestSeason.PointsAgainst, _exponent);
            var guestExpPct = go / (go + gd);
            guestSeason.ExpectedWins.ShouldBe(guestExpPct * guestSeason.Games);
            guestSeason.ExpectedLosses.ShouldBe((1m - guestExpPct) * guestSeason.Games);

            hostSeason.Games.ShouldBe(1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(hostScore);
            hostSeason.PointsAgainst.ShouldBe(guestScore);
            var ho = (decimal)Math.Pow(guestSeason.PointsFor, _exponent);
            var hd = (decimal)Math.Pow(guestSeason.PointsAgainst, _exponent);
            var hostExpPct = ho / (ho + hd);
            guestSeason.ExpectedWins.ShouldBe(hostExpPct * guestSeason.Games);
            guestSeason.ExpectedLosses.ShouldBe((1m - hostExpPct) * guestSeason.Games);

            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGame_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testStrategy.ProcessGame(null!));
        }

        [Fact(Skip = "This test will be skipped until it's necessary to verify that each game includes only member teams.")]
        public void ProcessGame_WhenGuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason { TeamId = 1 };
            var hostSeason = new TeamSeason { TeamId = 2 };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored))
                .Returns(new List<TeamSeason>{ guestSeason, hostSeason });

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).Returns(null);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            Assert.Throws<EntityNotFoundException>(() => testStrategy.ProcessGame(game));

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(guestSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(hostSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustNotHaveHappened();
        }

        [Fact(Skip = "This test will be skipped until it's necessary to verify that each game includes only member teams.")]
        public void ProcessGame_WhenHostSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason { TeamId = 1 };
            var hostSeason = new TeamSeason { TeamId = 2 };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored))
                .Returns(new List<TeamSeason> { guestSeason, hostSeason });

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).ReturnsNextFromSequence([guest, null]);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = guestName,
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            Assert.Throws<EntityNotFoundException>(() => testStrategy.ProcessGame(game));

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(guestSeason.TeamId)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeam(hostSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(1, 1, 0, 0, 0, 0, 1)]
        [InlineData(2, 1, 1, 0, 0, 1, 0)]
        [InlineData(1, 2, 0, 1, 1, 0, 0)]
        public async Task ProcessGameAsync_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int guestScore, int hostScore, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason
            {
                TeamId = 1,
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };
            var hostSeason = new TeamSeason
            {
                TeamId = 2,
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns([guestSeason, hostSeason]);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            var hostName = "Host";
            var host = new Team { Name = hostName };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored))
                .ReturnsNextFromSequence([guest, guest, host, host]);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = guestName,
                GuestScore = guestScore,
                HostName = hostName,
                HostScore = hostScore
            };
            await testStrategy.ProcessGameAsync(game);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(guestSeason.TeamId)).MustHaveHappened();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(hostSeason.TeamId)).MustHaveHappened();

            guestSeason.Games.ShouldBe(1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(guestScore);
            guestSeason.PointsAgainst.ShouldBe(hostScore);
            var go = (decimal)Math.Pow(guestSeason.PointsFor, _exponent);
            var gd = (decimal)Math.Pow(guestSeason.PointsAgainst, _exponent);
            var guestExpPct = go / (go + gd);
            guestSeason.ExpectedWins.ShouldBe(guestExpPct * guestSeason.Games);
            guestSeason.ExpectedLosses.ShouldBe((1m - guestExpPct) * guestSeason.Games);

            hostSeason.Games.ShouldBe(1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(hostScore);
            hostSeason.PointsAgainst.ShouldBe(guestScore);
            var ho = (decimal)Math.Pow(guestSeason.PointsFor, _exponent);
            var hd = (decimal)Math.Pow(guestSeason.PointsAgainst, _exponent);
            var hostExpPct = ho / (ho + hd);
            guestSeason.ExpectedWins.ShouldBe(hostExpPct * guestSeason.Games);
            guestSeason.ExpectedLosses.ShouldBe((1m - hostExpPct) * guestSeason.Games);

            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ProcessGameAsync_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testStrategy.ProcessGameAsync(null!).GetAwaiter().GetResult());
        }

        [Fact(Skip = "This test will be skipped until it's necessary to verify that each game includes only member teams.")]
        public async Task ProcessGameAsync_WhenGuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason { TeamId = 1 };
            var hostSeason = new TeamSeason { TeamId = 2 };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns([guestSeason, hostSeason]);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns<Team?>(null);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await testStrategy.ProcessGameAsync(game));

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(guestSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(hostSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustNotHaveHappened();
        }

        [Fact(Skip = "This test will be skipped until it's necessary to verify that each game includes only member teams.")]
        public async Task ProcessGameAsync_WhenHostSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason { TeamId = 1 };
            var hostSeason = new TeamSeason { TeamId = 2 };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>?>([guestSeason, hostSeason]));

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored))
                .ReturnsNextFromSequence([Task.FromResult(guest), Task.FromResult<Team>(null!)]);

            var testStrategy = new AddGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

            // Act
            var game = new Game
            {
                SeasonId = 1920,
                GuestName = guestName,
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            await Assert.ThrowsAsync<EntityNotFoundException>(async () => await testStrategy.ProcessGameAsync(game));

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(guestSeason.TeamId)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(hostSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustNotHaveHappened();
        }
    }
}
