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
    public class SubtractGameStrategyTest
    {
        private readonly ITeamRepository _fakeTeamRepository;
        private readonly ITeamSeasonRepository _fakeTeamSeasonRepository;
        private readonly SubtractGameStrategy _testStrategy;

        public SubtractGameStrategyTest()
        {
            _fakeTeamRepository = A.Fake<ITeamRepository>();
            _fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _testStrategy = new SubtractGameStrategy(_fakeTeamRepository, _fakeTeamSeasonRepository);
        }

        [Theory]
        [InlineData(1, 1, 0, 0, 1, 0, 2, 1, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 1, 1, 0, 0, 1, 2, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0)]
        //[InlineData(3, 1, 1, 1, 1, 1, 2, 1, 0, 1, 1, 0, 1)]
        //[InlineData(3, 1, 1, 1, 1, 1, 1, 2, 1, 0, 0, 1, 1)]
        [InlineData(3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0)]
        public void ProcessGame_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(int games, 
            int guestWins, int guestLosses, int hostWins, int hostLosses, int ties, int guestScore, int hostScore,
            int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            int points = 4;
            var guestSeason = new TeamSeason
            {
                TeamId = 1,
                Games = games,
                Wins = guestWins,
                Losses = guestLosses,
                Ties = ties,
                PointsFor = points,
                PointsAgainst = points
            };
            var hostSeason = new TeamSeason
            {
                TeamId = 2,
                Games = games,
                Wins = hostWins,
                Losses = hostLosses,
                Ties = ties,
                PointsFor = points,
                PointsAgainst = points
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored))
                .Returns([guestSeason, hostSeason]);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            var hostName = "Host";
            var host = new Team { Name = hostName };
            A.CallTo(() => fakeTeamRepository.GetTeam(An<int>.Ignored)).ReturnsNextFromSequence([guest, guest, host, host]);

            var testStrategy = new SubtractGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

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

            guestSeason.Games.ShouldBe(games - 1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(points - guestScore);
            guestSeason.PointsAgainst.ShouldBe(points - hostScore);
            guestSeason.ExpectedWins.ShouldBe(expGuestWins);
            guestSeason.ExpectedLosses.ShouldBe(expGuestLosses);

            hostSeason.Games.ShouldBe(games - 1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(points - hostScore);
            hostSeason.PointsAgainst.ShouldBe(points - guestScore);
            hostSeason.ExpectedWins.ShouldBe(expHostWins);
            hostSeason.ExpectedLosses.ShouldBe(expHostLosses);

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
            Assert.Throws<ArgumentNullException>(() => _testStrategy.ProcessGame(null!));
        }

        [Fact(Skip = "This test will be skipped until it's necessary to verify that each game includes only member teams.")]
        public void ProcessGame_WhenGuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestSeason = new TeamSeason { TeamId = 1 };
            var hostSeason = new TeamSeason { TeamId = 2 };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeason(An<int>.Ignored))
                .Returns([guestSeason, hostSeason]);

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
                .Returns([guestSeason, hostSeason]);

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
        [InlineData(1, 1, 0, 0, 1, 0, 2, 1, true, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 1, 1, 0, 0, 1, 2, true, 0, 0, 0, 0, 0)]
        [InlineData(1, 0, 0, 0, 0, 1, 1, 1, true, 0, 0, 0, 0, 0)]
        //[InlineData(3, 1, 1, 1, 1, 1, 2, 1, false, 0, 1, 1, 0, 1)]
        //[InlineData(3, 1, 1, 1, 1, 1, 1, 2, false, 1, 0, 0, 1, 1)]
        [InlineData(3, 1, 1, 1, 1, 1, 1, 1, true, 1, 1, 1, 1, 0)]
        public async Task ProcessGameAsync_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int games, int guestWins, int guestLosses, int hostWins, int hostLosses, int ties, int guestScore,
            int hostScore, bool isTie, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses,
            int expTies
            )
        {
            // Arrange
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            int points = 4;
            var guestSeason = new TeamSeason
            {
                TeamId = 1,
                Games = games,
                Wins = guestWins,
                Losses = guestLosses,
                Ties = ties,
                PointsFor = points,
                PointsAgainst = points
            };
            var hostSeason = new TeamSeason
            {
                TeamId = 2,
                Games = games,
                Wins = hostWins,
                Losses = hostLosses,
                Ties = ties,
                PointsFor = points,
                PointsAgainst = points
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(new List<TeamSeason> { guestSeason, hostSeason });

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            var hostName = "Host";
            var host = new Team { Name = hostName };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored))
                .ReturnsNextFromSequence([guest, guest, host, host]);

            var testStrategy = new SubtractGameStrategy(fakeTeamRepository, fakeTeamSeasonRepository);

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

            guestSeason.Games.ShouldBe(games - 1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(points - guestScore);
            guestSeason.PointsAgainst.ShouldBe(points - hostScore);
            guestSeason.ExpectedWins.ShouldBe(expGuestWins);
            guestSeason.ExpectedLosses.ShouldBe(expGuestLosses);

            hostSeason.Games.ShouldBe(games - 1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(points - hostScore);
            hostSeason.PointsAgainst.ShouldBe(points - guestScore);
            hostSeason.ExpectedWins.ShouldBe(expHostWins);
            hostSeason.ExpectedLosses.ShouldBe(expHostLosses);

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
            Assert.Throws<ArgumentNullException>(() => _testStrategy.ProcessGameAsync(null!).GetAwaiter().GetResult());
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
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns<Team>(null!);

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
                .Returns([guestSeason, hostSeason]);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var guestName = "Guest";
            var guest = new Team { Name = guestName };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).ReturnsNextFromSequence([guest, null]);

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
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(game.SeasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(guestSeason.TeamId)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(hostSeason.TeamId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(guestSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(hostSeason)).MustNotHaveHappened();
        }
    }
}
