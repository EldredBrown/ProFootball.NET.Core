using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class WeeklyUpdateServiceTest
    {
        public WeeklyUpdateServiceTest() {}

        [Theory]
        [InlineData(1919)]
        [InlineData(0)]
        public async Task RunWeeklyUpdate_WhenSeasonIdEqualsZero_ShouldThrowArgumentOutOfRangeException(int seasonId)
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
            var leagueId = 1;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => testService.RunWeeklyUpdate(leagueId, seasonId));
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenSeasonIdEqualsFirstSeasonIdAndLeagueSeasonIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(null));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(null));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotNullAndLeagueSeasonTotalsIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(null));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsIsNotNullAndLeagueSeasonTotalsTotalGamesIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = null
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalGamesIsNotNullAndTotalPointsIsNullAndDestSeasonIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = null
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalPointsIsNotNullAndDestSeasonIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(null));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(0);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1)]
        [InlineData(0)]
        public async Task RunWeeklyUpdate_WhenDestSeasonIsNotNullAndWeekCountIsLessThanThree_ShouldUpdateLeagueSeasonAndWeekCount(
            int maxWeek
        )
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountIsThreeAndTeamSeasonsIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(null!));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonsIsEmpty_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason> { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonsIsNotEmptyAndTeamSeasonScheduleTotalsIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", null!
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsEmpty_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object> { }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsNotEmptyAndTotalsScheduleGamesIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", null! }
                    }
                },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonTotalsScheduleGamesIsNotNullAndTotalScheduleAveragesIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", null!
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTotalScheduleAveragesIsEmpty_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object> { }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenAveragePointsAgainstIsNotNullAndLeagueSeasonIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 1m },
                        { "avg_points_against", 1m }
                    }
                },
                {
                    "LeagueSeason", null!
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsEmpty_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 1m },
                        { "avg_points_against", 1m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object> { }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotEmptyAndLeagueSeasonAveragePointsIsNull_ShouldUpdateLeagueSeasonAndWeekCount()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 1m },
                        { "avg_points_against", 1m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "average_points", null! }
                    }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonAveragePointsIsNotNullAndTeamSeasonGamesIsZero_ShouldUpdateLeagueSeasonAndWeekCountAndTeamSeason()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason
            {
                Id = 1,
                Games = 0
            };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 1m },
                        { "avg_points_against", 1m }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "average_points", 1m }
                    }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            teamSeason.OffensiveAverage.ShouldBeNull();
            teamSeason.OffensiveFactor.ShouldBeNull();
            teamSeason.OffensiveIndex.ShouldBeNull();
            teamSeason.DefensiveAverage.ShouldBeNull();
            teamSeason.DefensiveFactor.ShouldBeNull();
            teamSeason.DefensiveIndex.ShouldBeNull();
            teamSeason.FinalExpectedWinningPercentage.ShouldBeNull();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        public struct TeamSeasonData
        {
            public int Games { get; }
            public int PointsFor { get; }
            public int PointsAgainst { get; }

            public TeamSeasonData(int games, int pointsFor, int pointsAgainst)
            {
                Games = games;
                PointsFor = pointsFor;
                PointsAgainst = pointsAgainst;
            }
        }

        public struct TeamSeasonScheduleData
        {
            public double AveragePointsFor { get; }
            public double AveragePointsAgainst { get; }
            public TeamSeasonScheduleData(double averagePointsFor, double averagePointsAgainst)
            {
                AveragePointsFor = averagePointsFor;
                AveragePointsAgainst = averagePointsAgainst;
            }
        }

        public struct ExpectedOffensiveValues
        {
            public double? Average { get; }
            public double? Factor { get; }
            public double? Index { get; }
            public ExpectedOffensiveValues(double? average, double? factor, double? index)
            {
                Average = average;
                Factor = factor;
                Index = index;
            }
        }

        public struct ExpectedDefensiveValues
        {
            public double? Average { get; }
            public double? Factor { get; }
            public double? Index { get; }
            public ExpectedDefensiveValues(double? average, double? factor, double? index)
            {
                Average = average;
                Factor = factor;
                Index = index;
            }
        }

        public static IEnumerable<object[]> TestCases => new[]
        {
            new object[]
            {
                new TeamSeasonData(1, 0, 0),
                new TeamSeasonScheduleData(0d, 0d),
                0d,
                new ExpectedOffensiveValues(0d, null, null),
                new ExpectedDefensiveValues(0d, null, null),
                null
            },
            new object[]
            {
                new TeamSeasonData(1, 0, 0),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0d, 0d, 0d),
                new ExpectedDefensiveValues(0d, 0d, 0d),
                null
            },
            new object[]
            {
                new TeamSeasonData(1, 1, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0.5d
            },
            new object[]
            {
                new TeamSeasonData(1, 1, 0),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(0d, 0d, 0d),
                1d
            },
            new object[]
            {
                new TeamSeasonData(1, 0, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0d, 0d, 0d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0d
            },
            new object[]
            {
                new TeamSeasonData(1, 2, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(2d, 2d, 2d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0.8379d
            },
            new object[]
            {
                new TeamSeasonData(1, 1, 2),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(2d, 2d, 2d),
                0.1621d
            },
            new object[]
            {
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1.5d, 1.5d, 1.5d),
                new ExpectedDefensiveValues(0.5d, 0.5d, 0.5d),
                0.9311d
            },
            new object[]
            {
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0.5d, 0.5d, 0.5d),
                new ExpectedDefensiveValues(1.5d, 1.5d, 1.5d),
                0.0689d
            },
            new object[]
            {
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1.5d, 0.5d),
                1d,
                new ExpectedOffensiveValues(1.5d, 3.0d, 2.25d),
                new ExpectedDefensiveValues(0.5d, 0.3333d, 0.4167d),
                0.9820d
            },
            new object[]
            {
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(0.5d, 1.5d),
                1d,
                new ExpectedOffensiveValues(0.5d, 0.3333d, 0.4167d),
                new ExpectedDefensiveValues(1.5d, 3.0d, 2.25d),
                0.0180d
            },
            new object[]
            {
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1.5d, 0.5d),
                0.5d,
                new ExpectedOffensiveValues(1.5d, 3.0d, 1.5d),
                new ExpectedDefensiveValues(0.5d, 0.3333d, 0.3333d),
                0.9725d
            },
            new object[]
            {
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(0.5d, 1.5d),
                0.5d,
                new ExpectedOffensiveValues(0.5d, 0.3333d, 0.3333d),
                new ExpectedDefensiveValues(1.5d, 3.0d, 1.5d),
                0.0275d
            },
            new object[]
            {
                new TeamSeasonData(2, 35, 21),
                new TeamSeasonScheduleData(14d, 14d),
                14d,
                new ExpectedOffensiveValues(17.5d, 1.25d, 17.5d),
                new ExpectedDefensiveValues(10.5d, 0.75d, 10.5d),
                0.7704d
            },
            new object[]
            {
                new TeamSeasonData(2, 21, 35),
                new TeamSeasonScheduleData(14d, 14d),
                14d,
                new ExpectedOffensiveValues(10.5d, 0.75d, 10.5d),
                new ExpectedDefensiveValues(17.5d, 1.25d, 17.5d),
                0.2296d
            },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunWeeklyUpdate_WhenTeamSeasonGamesIsGreaterThanZero_ShouldUpdateLeagueSeasonAndWeekCountAndTeamSeason(
            TeamSeasonData teamSeasonData, TeamSeasonScheduleData teamSeasonScheduleData,
            double leagueSeasonAveragePoints,
            ExpectedOffensiveValues expectedOffensiveValues, ExpectedDefensiveValues expectedDefensiveValues,
            double? expTeamSeasonFinalExpectedWinningPercentage
        )
        {
            const decimal tolerance = 0.0001m;

            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<Season?>(season));

            var fakeGameRepository = A.Fake<IGameRepository>();
            var maxWeek = 3;
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(maxWeek);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { Id = 1 };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(Task.FromResult<LeagueSeason?>(leagueSeason));

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(Task.FromResult<LeagueSeasonTotals?>(leagueSeasonTotals));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason
            {
                Id = 1,
                Games = teamSeasonData.Games,
                PointsFor = teamSeasonData.PointsFor,
                PointsAgainst = teamSeasonData.PointsAgainst
            };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .Returns(Task.FromResult<IEnumerable<TeamSeason>>(teamSeasons));

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", (decimal)teamSeasonScheduleData.AveragePointsFor },
                        { "avg_points_against", (decimal)teamSeasonScheduleData.AveragePointsAgainst }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "average_points", (decimal)leagueSeasonAveragePoints }
                    }
                }
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .Returns(rankingsData);
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testService = new WeeklyUpdateService(fakeSeasonRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeTeamSeasonRepository, fakeLeagueSeasonTotalsRepository,
                fakeSeasonRankingsRepository, fakeSharedRepository);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored,
                An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            season.NumOfWeeksCompleted.ShouldBe(maxWeek);
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();

            if (expectedOffensiveValues.Average.HasValue)
            {
                teamSeason.OffensiveAverage.Value.ShouldBe((decimal)expectedOffensiveValues.Average, tolerance);
            }
            else
            {
                teamSeason.OffensiveAverage.ShouldBeNull();
            }

            if (expectedOffensiveValues.Factor.HasValue)
            {
                teamSeason.OffensiveFactor.Value.ShouldBe((decimal)expectedOffensiveValues.Factor, tolerance);
            }
            else
            {
                teamSeason.OffensiveFactor.ShouldBeNull();
            }

            if (expectedOffensiveValues.Index.HasValue)
            {
                teamSeason.OffensiveIndex.Value.ShouldBe((decimal)expectedOffensiveValues.Index, tolerance);
            }
            else
            {
                teamSeason.OffensiveIndex.ShouldBeNull();
            }

            if (expectedDefensiveValues.Average.HasValue)
            {
                teamSeason.DefensiveAverage.Value.ShouldBe((decimal)expectedDefensiveValues.Average, tolerance);
            }
            else
            {
                teamSeason.DefensiveAverage.ShouldBeNull();
            }

            if (expectedDefensiveValues.Factor.HasValue)
            {
                teamSeason.DefensiveFactor.Value.ShouldBe((decimal)expectedDefensiveValues.Factor, tolerance);
            }
            else
            {
                teamSeason.DefensiveFactor.ShouldBeNull();
            }

            if (expectedDefensiveValues.Index.HasValue)
            {
                teamSeason.DefensiveIndex.Value.ShouldBe((decimal)expectedDefensiveValues.Index, tolerance);
            }
            else
            {
                teamSeason.DefensiveIndex.ShouldBeNull();
            }

            if (expTeamSeasonFinalExpectedWinningPercentage.HasValue)
            {
                teamSeason.FinalExpectedWinningPercentage.Value
                    .ShouldBe((decimal)expTeamSeasonFinalExpectedWinningPercentage, tolerance);
            }
            else
            {
                teamSeason.FinalExpectedWinningPercentage.ShouldBeNull();
            }

            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }
    }
}
