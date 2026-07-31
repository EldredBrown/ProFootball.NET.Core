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
        public WeeklyUpdateServiceTest() { }

        [Theory]
        [InlineData(1919)]
        [InlineData(0)]
        public async Task RunWeeklyUpdate_WhenSeasonYearEqualsZero_ShouldThrowArgumentOutOfRangeException(int seasonYear)
        {
            // Arrange
            WeeklyUpdateService testService = SetUp();

            // Act and Assert
            var leagueId = 1;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => testService.RunWeeklyUpdate(leagueId, seasonYear));
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenSeasonYearEqualsFirstSeasonYearAndLeagueSeasonAndLeagueSeasonTotalsAreNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var weekCount = 0;
            WeeklyUpdateService testService = SetUp(weekCount: weekCount);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => testService._leagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotNullAndLeagueSeasonTotalsIsNull_ShouldNotUpdateAnything()
        {
            // Arrange
            var weekCount = 0;

            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };

            LeagueSeasonTotals? leagueSeasonTotals = null;

            WeeklyUpdateService testService = 
                SetUp(weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => testService._leagueSeasonRepository.Update(A<LeagueSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(null, null)]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsIsNotNullAndLeagueSeasonTotalGamesOrPointsIsNull_ShouldNotUpdateAnything(
            int? leagueSeasonTotalGames, int? leagueSeasonTotalPoints
        )
        {
            // Arrange
            var weekCount = 0;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = leagueSeasonTotalGames,
                TotalPoints = leagueSeasonTotalPoints
            };

            WeeklyUpdateService testService = 
                SetUp(weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals);

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1)]
        [InlineData(0)]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalGamesIsNotNullAndWeekCountIsLessThanThree_ShouldUpdateLeagueSeason(
            int weekCount
        )
        {
            // Arrange
            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            WeeklyUpdateService testService =
                SetUp(weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals);

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountIsThreeAndTeamSeasonsIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            List<TeamSeason>? teamSeasons = null;

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonsIsEmpty_ShouldUpdateLeagueSeaso()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeasons = new List<TeamSeason>();

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => testService._teamSeasonRepository.Update(A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonsIsNotEmptyAndTeamSeasonScheduleTotalsIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", null!
                },
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsEmpty_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object> { }
                },
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsIsNotEmptyAndTotalsScheduleGamesIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", null! }
                    }
                },
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonTotalsScheduleGamesIsNotNullAndTotalScheduleAveragesIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", null!
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTotalScheduleAveragesIsEmpty_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object> { }
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Theory]
        [InlineData(0, null)]
        [InlineData(null, null)]
        public async Task RunWeeklyUpdate_WhenTotalScheduleAveragesIsNotEmpty_ShouldUpdateLeagueSeason(
            int? avgPointsFor, int? avgPointsAgainst
        )
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", avgPointsFor },
                        { "avg_points_against", avgPointsAgainst }
                    }
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenAveragePointsForAndAveragePointsAgainstAreNotNullAndLeagueSeasonIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 0 },
                        { "avg_points_against", 0 }
                    }
                },
                {
                    "LeagueSeason", null!
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsEmpty_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 0 },
                        { "avg_points_against", 0 }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object> { }
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNotEmptyAndLeagueSeasonAveragePointsIsNull_ShouldUpdateLeagueSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };
            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };

            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 0 }
                    }
                },
                {
                    "TeamSeasonScheduleAverages", new Dictionary<string, object>
                    {
                        { "avg_points_for", 0 },
                        { "avg_points_against", 0 }
                    }
                },
                {
                    "LeagueSeason", new Dictionary<string, object>
                    {
                        { "average_points", null! },
                    }
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonAveragePointsIsNotNullAndTeamSeasonGamesIsZero_ShouldUpdateLeagueSeasonAndTeamSeason()
        {
            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };

            var teamSeason = new TeamSeason
            {
                Id = 1,
                Games = 0
            };
            var teamSeasons = new List<TeamSeason>
            {
                teamSeason
            };

            var rankingsData = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "TeamSeasonScheduleTotals", new Dictionary<string, object>
                    {
                        { "schedule_games", 1m }
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
                        { "average_points", 1m },
                    }
                }
            };

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
                .MustHaveHappenedOnceExactly();
            teamSeason.OffensiveAverage.ShouldBeNull();
            teamSeason.OffensiveFactor.ShouldBeNull();
            teamSeason.OffensiveIndex.ShouldBeNull();
            teamSeason.DefensiveAverage.ShouldBeNull();
            teamSeason.DefensiveFactor.ShouldBeNull();
            teamSeason.DefensiveIndex.ShouldBeNull();
            teamSeason.FinalExpectedWinningPercentage.ShouldBeNull();
            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        public readonly struct TeamSeasonData(int games, int pointsFor, int pointsAgainst)
        {
            public int Games { get; } = games;
            public int PointsFor { get; } = pointsFor;
            public int PointsAgainst { get; } = pointsAgainst;
        }

        public readonly struct TeamSeasonScheduleData(double averagePointsFor, double averagePointsAgainst)
        {
            public double AveragePointsFor { get; } = averagePointsFor;
            public double AveragePointsAgainst { get; } = averagePointsAgainst;
        }

        public readonly struct ExpectedOffensiveValues(double? average, double? factor, double? index)
        {
            public double? Average { get; } = average;
            public double? Factor { get; } = factor;
            public double? Index { get; } = index;
        }

        public readonly struct ExpectedDefensiveValues(double? average, double? factor, double? index)
        {
            public double? Average { get; } = average;
            public double? Factor { get; } = factor;
            public double? Index { get; } = index;
        }

        public static IEnumerable<object[]> TestCases =>
        [
            [
                new TeamSeasonData(1, 0, 0),
                new TeamSeasonScheduleData(0d, 0d),
                0d,
                new ExpectedOffensiveValues(0d, null, null),
                new ExpectedDefensiveValues(0d, null, null),
                null
            ],
            [
                new TeamSeasonData(1, 0, 0),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0d, 0d, 0d),
                new ExpectedDefensiveValues(0d, 0d, 0d),
                null
            ],
            [
                new TeamSeasonData(1, 1, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0.5d
            ],
            [
                new TeamSeasonData(1, 1, 0),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(0d, 0d, 0d),
                1d
            ],
            [
                new TeamSeasonData(1, 0, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0d, 0d, 0d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0d
            ],
            [
                new TeamSeasonData(1, 2, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(2d, 2d, 2d),
                new ExpectedDefensiveValues(1d, 1d, 1d),
                0.8379d
            ],
            [
                new TeamSeasonData(1, 1, 2),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1d, 1d, 1d),
                new ExpectedDefensiveValues(2d, 2d, 2d),
                0.1621d
            ],
            [
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(1.5d, 1.5d, 1.5d),
                new ExpectedDefensiveValues(0.5d, 0.5d, 0.5d),
                0.9311d
            ],
            [
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(1d, 1d),
                1d,
                new ExpectedOffensiveValues(0.5d, 0.5d, 0.5d),
                new ExpectedDefensiveValues(1.5d, 1.5d, 1.5d),
                0.0689d
            ],
            [
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1.5d, 0.5d),
                1d,
                new ExpectedOffensiveValues(1.5d, 3.0d, 2.25d),
                new ExpectedDefensiveValues(0.5d, 0.3333d, 0.4167d),
                0.9820d
            ],
            [
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(0.5d, 1.5d),
                1d,
                new ExpectedOffensiveValues(0.5d, 0.3333d, 0.4167d),
                new ExpectedDefensiveValues(1.5d, 3.0d, 2.25d),
                0.0180d
            ],
            [
                new TeamSeasonData(2, 3, 1),
                new TeamSeasonScheduleData(1.5d, 0.5d),
                0.5d,
                new ExpectedOffensiveValues(1.5d, 3.0d, 1.5d),
                new ExpectedDefensiveValues(0.5d, 0.3333d, 0.3333d),
                0.9725d
            ],
            [
                new TeamSeasonData(2, 1, 3),
                new TeamSeasonScheduleData(0.5d, 1.5d),
                0.5d,
                new ExpectedOffensiveValues(0.5d, 0.3333d, 0.3333d),
                new ExpectedDefensiveValues(1.5d, 3.0d, 1.5d),
                0.0275d
            ],
            [
                new TeamSeasonData(2, 35, 21),
                new TeamSeasonScheduleData(14d, 14d),
                14d,
                new ExpectedOffensiveValues(17.5d, 1.25d, 17.5d),
                new ExpectedDefensiveValues(10.5d, 0.75d, 10.5d),
                0.7704d
            ],
            [
                new TeamSeasonData(2, 21, 35),
                new TeamSeasonScheduleData(14d, 14d),
                14d,
                new ExpectedOffensiveValues(10.5d, 0.75d, 10.5d),
                new ExpectedDefensiveValues(17.5d, 1.25d, 17.5d),
                0.2296d
            ],
        ];

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunWeeklyUpdate_WhenTeamSeasonGamesIsGreaterThanZero_ShouldUpdateLeagueSeasonAndTeamSeason(
            TeamSeasonData teamSeasonData, TeamSeasonScheduleData teamSeasonScheduleData,
            double leagueSeasonAveragePoints,
            ExpectedOffensiveValues expectedOffensiveValues, ExpectedDefensiveValues expectedDefensiveValues,
            double? expTeamSeasonFinalExpectedWinningPercentage
        )
        {
            const decimal tolerance = 0.0001m;

            // Arrange
            var weekCount = 3;

            var leagueId = 1;
            var seasonYear = 1920;
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = leagueId,
                SeasonYear = seasonYear
            };

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 1
            };

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

            WeeklyUpdateService testService = SetUp(
                weekCount: weekCount, leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals,
                teamSeasons: teamSeasons, rankingsData: rankingsData
            );

            // Act
            await testService.RunWeeklyUpdate(leagueId, seasonYear);

            // Assert
            A.CallTo(() => testService._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._gameRepository.GetMaxWeekForSeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            leagueSeason.NumOfWeeksCompleted.ShouldBe(weekCount);
            leagueSeason.TotalGames.ShouldBe(leagueSeasonTotals.TotalGames.Value);
            leagueSeason.TotalPoints.ShouldBe(leagueSeasonTotals.TotalPoints.Value);
            A.CallTo(() => testService._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason))
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

            A.CallTo(() => testService._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testService._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedTwiceExactly();
        }

        private static WeeklyUpdateService SetUp(
            int? weekCount = null, LeagueSeason? leagueSeason = null, LeagueSeasonTotals? leagueSeasonTotals = null,
            List<TeamSeason>? teamSeasons = null, Dictionary<string, Dictionary<string, object>>? rankingsData = null
        )
        {
            var fakeGameRepository = A.Fake<IGameRepository>();
            if (weekCount is not null)
            {
                A.CallTo(() => fakeGameRepository.GetMaxWeekForSeasonAsync(A<int>.Ignored)).Returns(weekCount.Value);
            }

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeason);

            var fakeLeagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            A.CallTo(() => fakeLeagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            if (rankingsData is not null)
            {
                A.CallTo(() => fakeSeasonRankingsRepository.GetDataForRankingsUpdate(A<TeamSeason>.Ignored))
                    .Returns(rankingsData);
            }

            var sharedRepository = A.Fake<ISharedRepository>();

            return new WeeklyUpdateService(
                fakeGameRepository, fakeLeagueSeasonRepository, fakeTeamSeasonRepository,
                fakeLeagueSeasonTotalsRepository, fakeSeasonRankingsRepository, sharedRepository
            );
        }
    }
}
