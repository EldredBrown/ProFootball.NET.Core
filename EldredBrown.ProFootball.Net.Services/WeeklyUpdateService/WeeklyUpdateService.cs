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
    public class WeeklyUpdateService : IWeeklyUpdateService
    {
        private const int _firstYear = 1920;
        private const int _minWeekCountForRankingsUpdate = 3;

        private readonly ISeasonRepository _seasonRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILeagueSeasonRepository _leagueSeasonRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ILeagueSeasonTotalsRepository _leagueSeasonTotalsRepository;
        private readonly ISeasonRankingsRepository _seasonRankingsRepository;
        private readonly ISharedRepository _sharedRepository;

        private readonly object _dbLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyUpdateService"/> class.
        /// </summary>
        /// <param name="seasonRepository">The repository by which <see cref="Season"/> data will be accessed.</param>
        /// <param name="gameRepository">The repository by which <see cref="Game"/> data will be accessed.</param>
        /// <param name="leagueSeasonRepository">The repository by which <see cref="LeagueSeason"/> data will be accessed.</param>
        /// <param name="teamSeasonRepository">The repository by which <see cref="TeamSeason"/> data will be accessed.</param>
        /// <param name="leagueSeasonTotalsRepository">The repository by which <see cref="LeagueSeasonTotals"/> data will be accessed.</param>
        /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
        public WeeklyUpdateService(
            ISeasonRepository seasonRepository,
            IGameRepository gameRepository,
            ILeagueSeasonRepository leagueSeasonRepository,
            ITeamSeasonRepository teamSeasonRepository,
            ILeagueSeasonTotalsRepository leagueSeasonTotalsRepository,
            ISeasonRankingsRepository seasonRankingsRepository,
            ISharedRepository sharedRepository
        )
        {
            _seasonRepository = seasonRepository;
            _gameRepository = gameRepository;
            _leagueSeasonRepository = leagueSeasonRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _leagueSeasonTotalsRepository = leagueSeasonTotalsRepository;
            _seasonRankingsRepository = seasonRankingsRepository;
            _sharedRepository = sharedRepository;
        }

        /// <summary>
        /// Runs a weekly update of the data store.
        /// <param name="seasonYear">The year of the season within which a weekly update will be run.</param>
        /// </summary>
        public async Task RunWeeklyUpdate(int leagueId, int seasonId)
        {
            if (seasonId < _firstYear)
            {
                throw new ArgumentOutOfRangeException(nameof(seasonId), $"seasonId must be a positive integer; got {seasonId}");
            }

            await UpdateLeagueSeason(leagueId, seasonId);
            var srcWeekCount = await UpdateWeekCount(seasonId);
            await _sharedRepository.SaveChangesAsync();

            if (srcWeekCount < _minWeekCountForRankingsUpdate)
            {
                return;
            }

            await UpdateRankings(seasonId);
        }

        private async Task UpdateLeagueSeason(int leagueId, int seasonId)
        {
            var data = await GetLeagueSeasonData(leagueId, seasonId);
            if (data is null)
            {
                return;
            }

            var leagueSeason = data.LeagueSeason;
            var leagueSeasonTotals = data.LeagueSeasonTotals;
            UpdateLeagueSeasonGamesAndPoints(leagueSeason, leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value);
            _leagueSeasonRepository.Update(leagueSeason);
        }

        private async Task<LeagueSeasonData?> GetLeagueSeasonData(int leagueId, int seasonId)
        {
            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonId);
            if (leagueSeason is null)
            {
                return null;
            }

            var leagueSeasonTotals = await _leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId);
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

        private void UpdateLeagueSeasonGamesAndPoints(LeagueSeason leagueSeason, int totalGames, int totalPoints)
        {
            leagueSeason.TotalGames = totalGames;
            leagueSeason.TotalPoints = totalPoints;
            leagueSeason.AveragePoints = totalGames != 0
                ? totalPoints / (decimal)totalGames
                : null;
        }

        private async Task<int> UpdateWeekCount(int seasonId)
        {
            var srcWeekCount = await _gameRepository.GetMaxWeekForSeasonAsync(seasonId);

            var destSeason = await _seasonRepository.GetSeasonAsync(seasonId);
            if (destSeason is not null)
            {
                destSeason.NumOfWeeksCompleted = srcWeekCount;
                _seasonRepository.Update(destSeason);
            }
            return srcWeekCount;
        }

        private async Task UpdateRankings(int seasonId)
        {
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId);
            if (teamSeasons.IsNullOrEmpty())
            {
                return;
            }

            foreach (var teamSeason in teamSeasons)
            {
                await UpdateRankingsForTeamSeason(teamSeason);
            }

            await _sharedRepository.SaveChangesAsync();
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

            _teamSeasonRepository.Update(teamSeason);
        }

        private async Task<RankingsData?> GetRankingsData(TeamSeason teamSeason)
        {
            var results = _seasonRankingsRepository.GetDataForRankingsUpdate(teamSeason);

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

        private TeamSeasonRankingsData GetTeamSeasonRankingsData(int points, int games,
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

        private void CalculateFinalExpectedWinningPercentage(TeamSeason teamSeason)
        {
            if (teamSeason.OffensiveIndex is null || teamSeason.DefensiveIndex is null)
            {
                teamSeason.FinalExpectedWinningPercentage = null;
                return;
            }

            teamSeason.FinalExpectedWinningPercentage = MathUtils.CalculateExpectedWinningPercentage(
                teamSeason.OffensiveIndex.Value, teamSeason.DefensiveIndex.Value);
        }

        private class LeagueSeasonData
        {
            public LeagueSeasonData(LeagueSeason leagueSeason, LeagueSeasonTotals leagueSeasonTotals)
            {
                LeagueSeason = leagueSeason;
                LeagueSeasonTotals = leagueSeasonTotals;
            }

            public LeagueSeason LeagueSeason { get; }
            public LeagueSeasonTotals LeagueSeasonTotals { get; }
        }

        private class RankingsData
        {
            public RankingsData(Dictionary<string, object> averages, Dictionary<string, object> leagueSeason)
            {
                Averages = averages;
                LeagueSeason = leagueSeason;
            }

            public Dictionary<string, object> Averages { get; }
            public Dictionary<string, object> LeagueSeason { get; }
        }

        private class TeamSeasonRankingsData
        {
            public TeamSeasonRankingsData(decimal? average, decimal? factor, decimal? index)
            {
                Average = average;
                Factor = factor;
                Index = index;
            }

            public decimal? Average { get; set; }
            public decimal? Factor { get; set; }
            public decimal? Index { get; set; }
        }
    }
}
