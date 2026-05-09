using FakeItEasy;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;
using EldredBrown.ProFootball.Net.Services.Tests;

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

        [Fact]
        public void ProcessGame_WhenGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _testStrategy.ProcessGame(null!));
        }

        [Fact]
        public void ProcessGame_WhenGameArgIsNotNullAndGuestSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<IGameDecorator>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            TeamSeason? guestSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
                .Returns(guestSeason);

            // Act
            Assert.Throws<EntityNotFoundException>(() => _testStrategy.ProcessGame(fakeGame));

            // Assert
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.GuestName, fakeGame.SeasonYear))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ProcessGame_WhenGuestSeasonIsFoundAndHostSeasonIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var fakeGame = A.Fake<IGameDecorator>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.HostName = "Host";

            var guestSeason = A.Fake<TeamSeason>();
            TeamSeason? hostSeason = null;
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
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
        [InlineData(1, 1, true, 0, 0, 0, 0, 1)]
        [InlineData(2, 1, false, 1, 0, 0, 1, 0)]
        [InlineData(1, 2, false, 0, 1, 1, 0, 0)]
        public void ProcessGame_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int guestScore, int hostScore, bool isTie, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var fakeGame = A.Fake<IGameDecorator>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.GuestScore = guestScore;
            fakeGame.HostName = "Host";
            fakeGame.HostScore = hostScore;
            A.CallTo(() => fakeGame.IsTie).Returns(isTie);

            var guestSeason = new TeamSeason
            {
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };

            var hostSeason = new TeamSeason
            {
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            _testStrategy.ProcessGame(fakeGame);

            // Assert
            var seasonYear = fakeGame.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(fakeGame.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();

            guestSeason.Games.ShouldBe(1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(guestScore);
            guestSeason.PointsAgainst.ShouldBe(hostScore);
            //guestSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(guestScore, hostScore));

            hostSeason.Games.ShouldBe(1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(hostScore);
            hostSeason.PointsAgainst.ShouldBe(guestScore);
            //hostSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(hostScore, guestScore));

            A.CallTo(() => _teamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1, 1, true, 0, 0, 0, 0, 1)]
        [InlineData(2, 1, false, 1, 0, 0, 1, 0)]
        [InlineData(1, 2, false, 0, 1, 1, 0, 0)]
        public async Task ProcessGameAsync_WhenGuestAndHostSeasonsFound_ShouldUpdateTeamSeasonsWithCorrectData(
            int guestScore, int hostScore, bool isTie, int expGuestWins, int expGuestLosses, int expHostWins, int expHostLosses, int expTies)
        {
            // Arrange
            var fakeGame = A.Fake<IGameDecorator>();
            fakeGame.SeasonYear = 1920;
            fakeGame.GuestName = "Guest";
            fakeGame.GuestScore = guestScore;
            fakeGame.HostName = "Host";
            fakeGame.HostScore = hostScore;
            A.CallTo(() => fakeGame.IsTie).Returns(isTie);

            var guestSeason = new TeamSeason
            {
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };

            var hostSeason = new TeamSeason
            {
                Games = 0,
                Wins = 0,
                Losses = 0,
                Ties = 0,
                PointsFor = 0,
                PointsAgainst = 0
            };

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(A<string>.Ignored, A<int>.Ignored))
                .ReturnsNextFromSequence(guestSeason, hostSeason);

            // Act
            await _testStrategy.ProcessGameAsync(fakeGame);

            // Assert
            var seasonYear = fakeGame.SeasonYear;

            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(fakeGame.GuestName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(fakeGame.HostName, seasonYear))
                .MustHaveHappenedOnceExactly();

            guestSeason.Games.ShouldBe(1);
            guestSeason.Wins.ShouldBe(expGuestWins);
            guestSeason.Losses.ShouldBe(expGuestLosses);
            guestSeason.Ties.ShouldBe(expTies);
            guestSeason.PointsFor.ShouldBe(guestScore);
            guestSeason.PointsAgainst.ShouldBe(hostScore);
            //guestSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(guestScore, hostScore));

            hostSeason.Games.ShouldBe(1);
            hostSeason.Wins.ShouldBe(expHostWins);
            hostSeason.Losses.ShouldBe(expHostLosses);
            hostSeason.Ties.ShouldBe(expTies);
            hostSeason.PointsFor.ShouldBe(hostScore);
            hostSeason.PointsAgainst.ShouldBe(guestScore);
            //hostSeason.ExpectedWins.ShouldBeCloseTo(ProcessGameUtils.CalculateExpectedWinningPercentage(hostScore, guestScore));

            A.CallTo(() => _teamSeasonRepository.Update(guestSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _teamSeasonRepository.Update(hostSeason)).MustHaveHappenedOnceExactly();
        }
    }
}
