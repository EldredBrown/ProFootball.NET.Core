using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.Utilities;

namespace EldredBrown.ProFootball.Net.Services
{
    /// <summary>
    /// A service to run a weekly update of the pro football data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WeeklyUpdateService"/> class.
    /// </remarks>
    /// <param name="seasonRepository">The repository by which <see cref="Season"/> data will be accessed.</param>
    /// <param name="gameRepository">The repository by which <see cref="Game"/> data will be accessed.</param>
    /// <param name="leagueSeasonRepository">The repository by which <see cref="LeagueSeason"/> data will be accessed.</param>
    /// <param name="teamSeasonRepository">The repository by which <see cref="TeamSeason"/> data will be accessed.</param>
    /// <param name="leagueSeasonTotalsRepository">The repository by which <see cref="LeagueSeasonTotals"/> data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    public class WeeklyUpdateService(
        IGameRepository gameRepository,
        ILeagueSeasonRepository leagueSeasonRepository,
        ITeamSeasonRepository teamSeasonRepository,
        ILeagueSeasonTotalsRepository leagueSeasonTotalsRepository,
        ISeasonRankingsRepository seasonRankingsRepository,
        ISharedRepository sharedRepository
    ) : IWeeklyUpdateService
    {
        private const int _firstYear = 1920;
        private const int _minWeekCountForRankingsUpdate = 3;
        private readonly object _dbLock = new();

        /// <summary>
        /// Runs a weekly update of the data store.
        /// <param name="seasonYear">The year of the season within which a weekly update will be run.</param>
        /// </summary>
        public async Task RunWeeklyUpdate(int leagueId, int seasonYear)
        {
            if (seasonYear < _firstYear)
            {
                throw new ArgumentOutOfRangeException(nameof(seasonYear), $"seasonYear must be an integer greater than or equal to {_firstYear}; got {seasonYear}");
            }

            var data = await GetLeagueSeasonData(leagueId, seasonYear);
            if (data is null)
            {
                return;
            }

            var srcWeekCount = await UpdateLeagueSeason(data);
            if (srcWeekCount < _minWeekCountForRankingsUpdate)
            {
                return;
            }

            await UpdateRankings(seasonYear);
        }

        private async Task<int> UpdateLeagueSeason(LeagueSeasonData data)
        {
            var leagueSeason = data.LeagueSeason;
            var leagueSeasonTotals = data.LeagueSeasonTotals;

            var weekCount = await UpdateWeekCount(leagueSeason);
            UpdateLeagueSeasonGamesAndPoints(leagueSeason, leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value);

            leagueSeasonRepository.Update(leagueSeason);
            await sharedRepository.SaveChangesAsync();

            return weekCount;
        }

        private async Task<LeagueSeasonData?> GetLeagueSeasonData(int leagueId, int seasonYear)
        {
            var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear);
            if (leagueSeason is null)
            {
                return null;
            }

            var leagueSeasonTotals = await leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonYear);
            if (
                leagueSeasonTotals is null
                || leagueSeasonTotals.TotalGames is null
                || leagueSeasonTotals.TotalPoints is null)
            {
                return null;
            }

            return await Task.FromResult(
                new LeagueSeasonData(leagueSeason: leagueSeason, leagueSeasonTotals: leagueSeasonTotals));
        }

        private static void UpdateLeagueSeasonGamesAndPoints(LeagueSeason leagueSeason, int totalGames, int totalPoints)
        {
            leagueSeason.TotalGames = totalGames;
            leagueSeason.TotalPoints = totalPoints;
            leagueSeason.AveragePoints = totalGames != 0
                ? totalPoints / (decimal)totalGames
                : null;
        }

        private async Task<int> UpdateWeekCount(LeagueSeason leagueSeason)
        {
            var srcWeekCount = await gameRepository.GetMaxWeekForSeasonAsync(leagueSeason.SeasonYear);
            leagueSeason?.NumOfWeeksCompleted = srcWeekCount;
            return srcWeekCount;
        }

        private async Task UpdateRankings(int seasonYear)
        {
            var teamSeasons = await teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear);
            if (teamSeasons.IsNullOrEmpty())
            {
                return;
            }

            foreach (var teamSeason in teamSeasons)
            {
                await UpdateRankingsForTeamSeason(teamSeason);
            }

            await sharedRepository.SaveChangesAsync();
        }

        private async Task UpdateRankingsForTeamSeason(TeamSeason teamSeason)
        {
            var data = await GetRankingsData(teamSeason);
            if (data is null)
            {
                return;
            }

            lock (_dbLock)
            {
                var offense = GetTeamSeasonRankingsData(teamSeason.PointsFor, teamSeason.Games,
                    (decimal)data.Averages["avg_points_against"], (decimal)data.LeagueSeason["average_points"]);
                teamSeason.OffensiveAverage = offense.Average;
                teamSeason.OffensiveFactor = offense.Factor;
                teamSeason.OffensiveIndex = offense.Index;

                var defense = GetTeamSeasonRankingsData(teamSeason.PointsAgainst, teamSeason.Games,
                    (decimal)data.Averages["avg_points_for"], (decimal)data.LeagueSeason["average_points"]);
                teamSeason.DefensiveAverage = defense.Average;
                teamSeason.DefensiveFactor = defense.Factor;
                teamSeason.DefensiveIndex = defense.Index;

                CalculateFinalExpectedWinningPercentage(teamSeason);
            }

            teamSeasonRepository.Update(teamSeason);
        }

        private async Task<RankingsData?> GetRankingsData(TeamSeason teamSeason)
        {
            var results = seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason);

            var totals = results["TeamSeasonScheduleTotals"];
            if (totals.IsNullOrEmpty() || totals["schedule_games"] is null)
            {
                return null;
            }

            var averages = results["TeamSeasonScheduleAverages"];
            if (averages.IsNullOrEmpty() || averages["avg_points_for"] is null || averages["avg_points_against"] is null)
            {
                return null;
            }

            var leagueSeason = results["LeagueSeason"];
            if (leagueSeason.IsNullOrEmpty() || leagueSeason["average_points"] is null)
            {
                return null;
            }

            return new RankingsData(averages: averages, leagueSeason: leagueSeason);
        }

        private static TeamSeasonRankingsData GetTeamSeasonRankingsData(int points, int games,
            decimal teamSeasonScheduleAveragePoints, decimal leagueSeasonAveragePoints)
        {
            if (games == 0)
            {
                return new TeamSeasonRankingsData(average: null, factor: null, index: null);
            }

            decimal? average = MathUtils.Divide(points, games);
            decimal? factor = MathUtils.Divide(average!.Value, teamSeasonScheduleAveragePoints);
            decimal? index = factor.HasValue
                ? (average.Value + factor.Value * leagueSeasonAveragePoints) / 2m
                : null;

            return new TeamSeasonRankingsData(average, factor, index);
        }

        private static void CalculateFinalExpectedWinningPercentage(TeamSeason teamSeason)
        {
            if (teamSeason.OffensiveIndex is null || teamSeason.DefensiveIndex is null)
            {
                teamSeason.FinalExpectedWinningPercentage = null;
                return;
            }

            teamSeason.FinalExpectedWinningPercentage = MathUtils.CalculateExpectedWinningPercentage(
                teamSeason.OffensiveIndex.Value, teamSeason.DefensiveIndex.Value);
        }

        private class LeagueSeasonData(LeagueSeason leagueSeason, LeagueSeasonTotals leagueSeasonTotals)
        {
            public LeagueSeason LeagueSeason { get; } = leagueSeason;
            public LeagueSeasonTotals LeagueSeasonTotals { get; } = leagueSeasonTotals;
        }

        private class RankingsData(Dictionary<string, object> averages, Dictionary<string, object> leagueSeason)
        {
            public Dictionary<string, object> Averages { get; } = averages;
            public Dictionary<string, object> LeagueSeason { get; } = leagueSeason;
        }

        private class TeamSeasonRankingsData(decimal? average, decimal? factor, decimal? index)
        {
            public decimal? Average { get; set; } = average;
            public decimal? Factor { get; set; } = factor;
            public decimal? Index { get; set; } = index;
        }
    }
}
