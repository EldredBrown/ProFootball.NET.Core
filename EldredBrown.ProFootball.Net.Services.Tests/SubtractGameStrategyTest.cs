using System;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class SubtractGameStrategyTest
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly SubtractGameStrategy _testStrategy;

        public SubtractGameStrategyTest()
        {
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _testStrategy = new SubtractGameStrategy(_teamSeasonRepository);
        }

        [Theory]
        [InlineData(1, 1, true, 1, 1, 1, 1, 0)]
        [InlineData(2, 1, false, 0, 1, 1, 0, 1)]
        [InlineData(1, 2, false, 1, 0, 0, 1, 1)]
        public void ProcessGame_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int guestScore, int hostScore, bool isTie, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var game = new Game
            {
                SeasonYear = 1920,
                GuestName = "Guest",
                GuestScore = guestScore,
                HostName = "Host",
                HostScore = hostScore
            };

            int points = 4;
            var guestSeason = new TeamSeason
            {
                Games = 3,
                Wins = 1,
                Losses = 1,
                Ties = 1,
                PointsFor = points,
                PointsAgainst = points
            };

            var hostSeason = new TeamSeason
            {
                Games = 3,
                Wins = 1,
                Losses = 1,
                Ties = 1,
                PointsFor = points,
                PointsAgainst = points
            };

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, An<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            _testStrategy.ProcessGame(game);

            // Assert
            var seasonYear = game.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(game.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(game.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();

            guestSeason.Games.ShouldBe(2);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(points - guestScore);
            guestSeason.PointsAgainst.ShouldBe(points - hostScore);
            //guestSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(guestScore, hostScore));

            hostSeason.Games.ShouldBe(2);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(points - hostScore);
            hostSeason.PointsAgainst.ShouldBe(points - guestScore);
            //hostSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(hostScore, guestScore));

            A.CallTo(() => _teamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGame_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _testStrategy.ProcessGame(null!));
        }

        [Fact]
        public void ProcessGame_GuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<Game>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            TeamSeason? guestSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, An<int>.Ignored))
                .Returns(guestSeason);

            // Act
            Assert.Throws<EntityNotFoundException>(() => _testStrategy.ProcessGame(fakeGame));

            // Assert
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.GuestName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGame_HostSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<Game>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            var guestSeason = A.Fake<TeamSeason>();
            TeamSeason? hostSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, An<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            Assert.Throws<EntityNotFoundException>(() => _testStrategy.ProcessGame(fakeGame));

            // Assert
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.GuestName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.HostName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1, 1, true, 1, 1, 1, 1, 0)]
        [InlineData(2, 1, false, 0, 1, 1, 0, 1)]
        [InlineData(1, 2, false, 1, 0, 0, 1, 1)]
        public async Task ProcessGameAsync_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int guestScore, int hostScore, bool isTie, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var game = new Game
            {
                SeasonYear = 1920,
                GuestName = "Guest",
                GuestScore = guestScore,
                HostName = "Host",
                HostScore = hostScore
            };

            int points = 4;
            var guestSeason = new TeamSeason
            {
                Games = 3,
                Wins = 1,
                Losses = 1,
                Ties = 1,
                PointsFor = points,
                PointsAgainst = points
            };

            var hostSeason = new TeamSeason
            {
                Games = 3,
                Wins = 1,
                Losses = 1,
                Ties = 1,
                PointsFor = points,
                PointsAgainst = points
            };

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            await _testStrategy.ProcessGameAsync(game);

            // Assert
            var seasonYear = game.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(game.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(game.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();

            guestSeason.Games.ShouldBe(2);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(points - guestScore);
            guestSeason.PointsAgainst.ShouldBe(points - hostScore);
            //guestSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(guestScore, hostScore));

            hostSeason.Games.ShouldBe(2);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(points - hostScore);
            hostSeason.PointsAgainst.ShouldBe(points - guestScore);
            //hostSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(hostScore, guestScore));

            A.CallTo(() => _teamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGameAsync_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _testStrategy.ProcessGameAsync(null!).GetAwaiter().GetResult());
        }

        [Fact]
        public void ProcessGameAsync_GuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<Game>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            TeamSeason? guestSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(guestSeason);

            // Act
            Assert.Throws<EntityNotFoundException>(() => _testStrategy.ProcessGameAsync(fakeGame).GetAwaiter().GetResult());

            // Assert
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(fakeGame.GuestName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGameAsync_HostSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<Game>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            var guestSeason = A.Fake<TeamSeason>();
            TeamSeason? hostSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            Assert.Throws<EntityNotFoundException>(() => _testStrategy.ProcessGameAsync(fakeGame).GetAwaiter().GetResult());

            // Assert
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(fakeGame.GuestName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(fakeGame.HostName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
        }
    }
}
