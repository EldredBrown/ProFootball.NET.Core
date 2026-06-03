using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using System;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class WeeklyUpdateServiceTest
    {
        public WeeklyUpdateServiceTest() {}

        [Fact]
        public async Task RunWeeklyUpdate_WhenSeasonYearIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, sharedRepository);

            // Act and Assert
            var leagueName = "APFA";
            var seasonYear = -1;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => testService.RunWeeklyUpdate(leagueName, seasonYear));
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenSeasonYearEqualsZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, sharedRepository);

            // Act and Assert
            var leagueName = "APFA";
            var seasonYear = 0;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => testService.RunWeeklyUpdate(leagueName, seasonYear));
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenSeasonYearIsGreaterThanZeroAndLeagueSeasonTotalsIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(null));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored))
                .Returns(null);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeason(A<string>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(An<int>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsIsNotNullAndLeagueSeasonTotalsTotalGamesIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(null));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = null,
                TotalPoints = null
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(An<int>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalGamesIsNotNullAndLeagueSeasonTotalsTotalPointsIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(null));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = null
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(An<int>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalPointsIsNotNullAndLeagueSeasonIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(null));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(An<int>.Ignored, An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenDestSeasonIsNotNull_ShouldUpdateWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountLessThanThree_ShouldNotUpdateRankings()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(2);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeason = A.Fake<ITeamSeason>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountIsThreeAndTeamSeasonsForSpecifiedYearIsEmpty_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(3);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountIsGreaterThanThreeAndTeamSeasonsForSpecifiedYearIsEmpty_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonsForSpecifiedYearIsNotEmptyAndTeamSeasonScheduleTotalsIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", null
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsEmpty_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object> { }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsNotEmptyAndTeamSeasonScheduleGamesIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", null }
                    }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleGamesIsNotNullAndTeamSeasonScheduleAveragesIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                { 
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", null
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleAveragesIsEmpty_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object> { }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleAveragesIsNotEmptyAndAvgPointsForIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", null }
                    }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenAvgPointsForIsNotNullAndAvgPointsAgainstIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", 0m },
                        { "PointsAgainst", null }
                    }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenAvgPointsAgainstIsNotNullAndLeagueSeasonIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", 0m },
                        { "PointsAgainst", 0m }
                    }
                },
                {
                    "LeagueSeason", null
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsEmpty_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", 0m },
                        { "PointsAgainst", 0m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object> { }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotEmptyAndAveragePointsIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", 0m },
                        { "PointsAgainst", 0m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "AveragePoints", null }
                    }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(A<decimal>.Ignored, A<decimal>.Ignored, A<decimal>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenAveragePointsIsNotNull_ShouldUpdateRankingsForTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeDestSeason = A.Fake<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(An<int>.Ignored)).Returns(Task.FromResult<Season?>(fakeDestSeason));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(4);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeLeagueSeason = A.Fake<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeason?>(fakeLeagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, An<int>.Ignored)).Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeason = A.Fake<TeamSeason>();
            var teamSeasons = new List<TeamSeason>(new List<TeamSeason> { fakeTeamSeason });
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var dataForRankingsUpdate = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "ScheduleGames", 0m }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "PointsFor", 0m },
                        { "PointsAgainst", 0m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "AveragePoints", 0m }
                    }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(A<ITeamSeason>.Ignored))
                .Returns(Task.FromResult(dataForRankingsUpdate));

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueName = "APFA";
            var seasonYear = 1;
            await testService.RunWeeklyUpdate(leagueName, seasonYear);

            // Assert
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(fakeLeagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonByYearAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(fakeDestSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdateAsync(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeason.UpdateRankings(
                (decimal)dataForRankingsUpdate["TeamSeasonScheduleAverages"]["PointsFor"],
                (decimal)dataForRankingsUpdate["TeamSeasonScheduleAverages"]["PointsAgainst"],
                (decimal)dataForRankingsUpdate["LeagueSeason"]["AveragePoints"]))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(fakeTeamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }
    }
}
