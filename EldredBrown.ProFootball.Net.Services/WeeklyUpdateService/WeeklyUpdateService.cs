//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//using Microsoft.IdentityModel.Tokens;

//using EldredBrown.ProFootball.Net.Data.Models;
//using EldredBrown.ProFootball.Net.Data.Repositories;

//namespace EldredBrown.ProFootball.Net.Services
//{
//    /// <summary>
//    /// A service to run a weekly update of the pro football data store.
//    /// </summary>
//    public class WeeklyUpdateService : IWeeklyUpdateService
//    {
//        private const int _minWeekCountForRankingsUpdate = 3;

//        private readonly ISeasonRepository _seasonRepository;
//        private readonly IGameRepository _gameRepository;
//        private readonly ILeagueSeasonRepository _leagueSeasonRepository;
//        private readonly ITeamSeasonRepository _teamSeasonRepository;
//        private readonly ILeagueSeasonTotalsRepository _leagueSeasonTotalsRepository;
//        private readonly ISeasonRankingsRepository _seasonRankingsRepository;
//        private readonly ISharedRepository _sharedRepository;

//        private readonly object _dbLock = new object();

//        /// <summary>
//        /// Initializes a new instance of the <see cref="WeeklyUpdateService"/> class.
//        /// </summary>
//        /// <param name="seasonRepository">The repository by which Season data will be accessed.</param>
//        /// <param name="gameRepository">The repository by which Game data will be accessed.</param>
//        /// <param name="leagueSeasonRepository">The repository by which LeagueSeason data will be accessed.</param>
//        /// <param name="teamSeasonRepository">The repository by which TeamSeason data will be accessed.</param>
//        /// <param name="leagueSeasonTotalsRepository">The repository by which LeagueSeasonTotals data will be accessed.</param>
//        /// <param name="teamSeasonScheduleRepository">The repository by which TeamSeason schedule data will be accessed.</param>
//        /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
//        public WeeklyUpdateService(
//            ISeasonRepository seasonRepository,
//            IGameRepository gameRepository,
//            ILeagueSeasonRepository leagueSeasonRepository,
//            ITeamSeasonRepository teamSeasonRepository,
//            ILeagueSeasonTotalsRepository leagueSeasonTotalsRepository,
//            ISeasonRankingsRepository seasonRankingsRepository,
//            ISharedRepository sharedRepository)
//        {
//            _seasonRepository = seasonRepository;
//            _gameRepository = gameRepository;
//            _leagueSeasonRepository = leagueSeasonRepository;
//            _teamSeasonRepository = teamSeasonRepository;
//            _leagueSeasonTotalsRepository = leagueSeasonTotalsRepository;
//            _seasonRankingsRepository = seasonRankingsRepository;
//            _sharedRepository = sharedRepository;
//        }

//        /// <summary>
//        /// Runs a weekly update of the data store.
//        /// <param name="seasonYear">The year of the season within which a weekly update will be run.</param>
//        /// </summary>
//        public async Task RunWeeklyUpdate(string leagueName, int seasonYear)
//        {
//            if (seasonYear <= 0)
//            {
//                throw new ArgumentOutOfRangeException(nameof(seasonYear), $"seasonYear must be a positive integer; got {seasonYear}");
//            }

//            await UpdateLeagueSeason(leagueName, seasonYear);
//            var srcWeekCount = await UpdateWeekCount(seasonYear);

//            await _sharedRepository.SaveChangesAsync();

//            if (srcWeekCount >= _minWeekCountForRankingsUpdate)
//            {
//                await UpdateRankings(seasonYear);
//            }
//        }

//        private async Task<LeagueSeasonData?> GetLeagueSeasonData(string leagueName, int seasonYear)
//        {
//            var leagueSeasonTotals = await _leagueSeasonTotalsRepository.GetLeagueSeasonTotalsAsync(leagueName, seasonYear);
//            if (
//                leagueSeasonTotals is null
//                || leagueSeasonTotals.TotalGames is null
//                || leagueSeasonTotals.TotalPoints is null)
//            {
//                return null;
//            }

//            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear);
//            if (leagueSeason is null)
//            {
//                return null;
//            }

//            return await Task.FromResult(new LeagueSeasonData(leagueSeasonTotals: leagueSeasonTotals,
//                leagueSeason: leagueSeason));
//        }

//        private async Task<RankingsData?> GetRankingsData(ITeamSeason teamSeason)
//        {
//            var results = await _seasonRankingsRepository.GetDataForRankingsUpdateAsync(teamSeason);

//            var totals = results["TeamSeasonScheduleTotals"];
//            if (totals.IsNullOrEmpty() || totals["ScheduleGames"] is null)
//            {
//                return null;
//            }

//            var averages = results["TeamSeasonScheduleAverages"];
//            if (averages.IsNullOrEmpty() || averages["PointsFor"] is null || averages["PointsAgainst"] is null)
//            {
//                return null;
//            }

//            var leagueSeason = results["LeagueSeason"];
//            if (leagueSeason.IsNullOrEmpty() || leagueSeason["AveragePoints"] is null)
//            {
//                return null;
//            }

//            return new RankingsData(averages: averages, leagueSeason: leagueSeason);
//        }

//        private async Task UpdateLeagueSeason(string leagueName, int seasonYear)
//        {
//            var data = await GetLeagueSeasonData(leagueName, seasonYear);
//            if (data is null)
//            {
//                return;
//            }

//            var leagueSeasonTotals = data.LeagueSeasonTotals;
//            var leagueSeason = data.LeagueSeason;
//            leagueSeason.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
//                leagueSeasonTotals.TotalPoints.Value);
//            _leagueSeasonRepository.Update(leagueSeason);
//        }

//        private async Task UpdateRankings(int seasonYear)
//        {
//            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear);
//            if (teamSeasons.IsNullOrEmpty())
//            {
//                return;
//            }

//            foreach (var teamSeason in teamSeasons)
//            {
//                await UpdateRankingsForTeamSeason(teamSeason);
//            }

//            await _sharedRepository.SaveChangesAsync();
//        }

//        private async Task UpdateRankingsForTeamSeason(TeamSeason teamSeason)
//        {
//            var data = await GetRankingsData(teamSeason);
//            if (data is null)
//            {
//                return;
//            }

//            lock (_dbLock)
//            {
//                teamSeason.UpdateRankings((decimal)data.Averages["PointsFor"], (decimal)data.Averages["PointsAgainst"],
//                    (decimal)data.LeagueSeason["AveragePoints"]);
//            }

//            _teamSeasonRepository.Update(teamSeason);
//        }

//        private async Task<int> UpdateWeekCount(int seasonYear)
//        {
//            var srcWeekCount = await _gameRepository.GetMaxWeekForSeasonAsync(seasonYear);

//            var destSeason = await _seasonRepository.GetSeasonByYearAsync(seasonYear);
//            if (destSeason is not null)
//            {
//                destSeason.NumOfWeeksCompleted = srcWeekCount;
//                _seasonRepository.Update(destSeason);
//            }
//            return srcWeekCount;
//        }

//        private class LeagueSeasonData
//        {
//            public LeagueSeasonData(LeagueSeasonTotals leagueSeasonTotals, LeagueSeason leagueSeason)
//            {
//                LeagueSeasonTotals = leagueSeasonTotals;
//                LeagueSeason = leagueSeason;
//            }

//            public LeagueSeasonTotals LeagueSeasonTotals { get; }
//            public LeagueSeason LeagueSeason { get; }
//        }

//        private class RankingsData
//        {
//            public RankingsData(Dictionary<string, object> averages, Dictionary<string, object> leagueSeason)
//            {
//                Averages = averages;
//                LeagueSeason = leagueSeason;
//            }

//            public Dictionary<string, object> Averages { get; }
//            public Dictionary<string, object> LeagueSeason { get; }
//        }
//    }
//}
