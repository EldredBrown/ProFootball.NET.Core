using System;
using System.Linq;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.NETCore.Services
{
    /// <summary>
    /// A service to run a weekly update of the pro football data store.
    /// </summary>
    public class WeeklyUpdateService : IWeeklyUpdateService
    {
        private const int _selectedSeason = 1920;

        private readonly ISeasonRepository _seasonRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILeagueSeasonRepository _leagueSeasonRepository;
        private readonly ILeagueSeasonTotalsRepository _leagueSeasonTotalsRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;
        private readonly ISharedRepository _sharedRepository;

        private readonly object _dbLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyUpdateService"/> class.
        /// </summary>
        /// <param name="seasonRepository">The repository by which Season data will be accessed.</param>
        /// <param name="gameRepository">The repository by which Game data will be accessed.</param>
        /// <param name="leagueSeasonRepository">The repository by which LeagueSeason data will be accessed.</param>
        /// <param name="leagueSeasonTotalsRepository">The repository by which LeagueSeasonTotals data will be accessed.</param>
        /// <param name="teamSeasonRepository">The repository by which TeamSeason data will be accessed.</param>
        /// <param name="teamSeasonScheduleRepository">The repository by which TeamSeason schedule data will be accessed.</param>
        /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
        public WeeklyUpdateService(
            ISeasonRepository seasonRepository,
            IGameRepository gameRepository,
            ILeagueSeasonRepository leagueSeasonRepository,
            ILeagueSeasonTotalsRepository leagueSeasonTotalsRepository,
            ITeamSeasonRepository teamSeasonRepository,
            ITeamSeasonScheduleRepository teamSeasonScheduleRepository,
            ISharedRepository sharedRepository)
        {
            _seasonRepository = seasonRepository;
            _gameRepository = gameRepository;
            _leagueSeasonRepository = leagueSeasonRepository;
            _leagueSeasonTotalsRepository = leagueSeasonTotalsRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _teamSeasonScheduleRepository = teamSeasonScheduleRepository;
            _sharedRepository = sharedRepository;
        }

        /// <summary>
        /// Runs a weekly update of the data store.
        /// <param name="seasonYear">The year of the season within which a weekly update will be run.</param>
        /// </summary>
        public async Task RunWeeklyUpdate(int seasonYear)
        {
            // These hard-coded values are a bit of a hack at this time, but I intend to make them selectable by the
            // user in the future.
            var leagueName = "APFA";

            await UpdateLeagueSeason(leagueName, seasonYear);
            var srcWeekCount = await UpdateWeekCount(seasonYear);

            await _sharedRepository.SaveChangesAsync();

            if (srcWeekCount >= 3)
            {
                await UpdateRankings();
            }
        }

        private async Task UpdateLeagueSeason(string leagueName, int seasonYear)
        {
            var leagueSeasonTotals = _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear);
            if (leagueSeasonTotals.TotalGames is null || leagueSeasonTotals.TotalPoints is null)
            {
                return;
            }

            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear);
            if (leagueSeason is null)
            {
                return;
            }

            var leagueSeasonDecorator = new LeagueSeasonDecorator(leagueSeason);
            leagueSeasonDecorator.UpdateGamesAndPoints(leagueSeasonTotals.TotalGames.Value,
                leagueSeasonTotals.TotalPoints.Value);
        }

        private async Task UpdateRankings()
        {
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(_selectedSeason);
            foreach (var teamSeason in teamSeasons)
            {
                var teamSeasonDecorator = new TeamSeasonDecorator(teamSeason);
                await UpdateRankingsForTeamSeason(teamSeasonDecorator);
            }

            await _sharedRepository.SaveChangesAsync();
        }

        private async Task UpdateRankingsForTeamSeason(TeamSeasonDecorator teamSeasonDecorator)
        {
            try
            {
                var teamSeasonScheduleTotals =
                    await _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(
                        teamSeasonDecorator.TeamName, teamSeasonDecorator.SeasonYear);
                if (teamSeasonScheduleTotals.ScheduleGames is null)
                {
                    return;
                }

                var teamSeasonScheduleAverages =
                    await _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(
                        teamSeasonDecorator.TeamName, teamSeasonDecorator.SeasonYear);
                if (teamSeasonScheduleAverages.PointsFor is null ||
                    teamSeasonScheduleAverages.PointsAgainst is null)
                {
                    return;
                }

                var leagueSeason =
                    await _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                        teamSeasonDecorator.LeagueName, teamSeasonDecorator.SeasonYear);
                if (leagueSeason?.AveragePoints is null)
                {
                    return;
                }

                lock (_dbLock)
                {
                    teamSeasonDecorator.UpdateRankings(teamSeasonScheduleAverages.PointsFor.Value,
                        teamSeasonScheduleAverages.PointsAgainst.Value, leagueSeason.AveragePoints.Value);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Nullable object must have a value.")
                {
                    // Ignore exception.
                }
                else
                {
                    //_sharedService.ShowExceptionMessage(ex, $"InvalidOperationException: {teamSeason.TeamName}");
                }
            }
            catch (Exception)
            {
                //_sharedService.ShowExceptionMessage(ex.InnerException, $"Exception: {teamSeason.TeamName}");
            }
        }

        private async Task<int> UpdateWeekCount(int seasonYear)
        {
            var srcWeekCount = (await _gameRepository.GetGamesAsync())
                .Where(g => g.SeasonYear == seasonYear)
                .Select(g => g.Week)
                .Max();

            var destSeason = await _seasonRepository.GetSeasonAsync(seasonYear);
            if (!(destSeason is null))
            {
                destSeason.NumOfWeeksCompleted = srcWeekCount;
            }

            return srcWeekCount;
        }
    }
}
