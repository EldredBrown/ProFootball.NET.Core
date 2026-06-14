using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Data;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamSeasonController"/> class.
    /// </summary>
    /// <param name="teamSeasonIndexViewModel">
    /// The <see cref="ITeamSeasonIndexViewModel"/> that will provide data to the TeamSeasons index view.
    /// </param>
    /// <param name="teamSeasonDetailsViewModel">
    /// The <see cref="ITeamSeasonDetailsViewModel"/> that will provide data to the TeamSeasons details view.
    /// </param>
    /// <param name="seasonRepository">
    /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="teamSeasonRepository">
    /// The <see cref="ITeamSeasonRepository"/> by which team season data will be accessed.
    /// </param>
    /// <param name="teamSeasonScheduleRepository">
    /// The <see cref="ITeamSeasonScheduleRepository"/> by which team season schedule data will be accessed.
    /// </param>
    /// <param name="weeklyUpdateService">
    /// The <see cref="IWeeklyUpdateService"/> that will run weekly updates of the data store.
    /// </param>
    public class TeamSeasonController(
        ITeamSeasonIndexViewModel teamSeasonIndexViewModel,
        ITeamSeasonDetailsViewModel teamSeasonDetailsViewModel,
        ITeamSeasonViewModelMapper teamSeasonViewModelMapper,
        ISeasonRepository seasonRepository,
        ITeamSeasonRepository teamSeasonRepository,
        ITeamSeasonScheduleRepository teamSeasonScheduleRepository,
        IWeeklyUpdateService weeklyUpdateService
        ) : Controller
    {
        // GET: TeamSeasons
        /// <summary>
        /// Renders a view of the team seasons index.
        /// </summary>
        /// <returns>The rendered view of the team seasons index.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var seasons = (await seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Id);

            var selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            if (selectedSeasonYear is null)
            {
                SetSelectedSeasonYear(seasons.First().Id);
                selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            }
            teamSeasonIndexViewModel.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);
            teamSeasonIndexViewModel.SelectedSeasonYear = selectedSeasonYear;

            var teamSeasons = await teamSeasonRepository.GetTeamSeasonsBySeasonAsync(selectedSeasonYear.Value);
            teamSeasonIndexViewModel.TeamSeasons = teamSeasons
                .Select(ts => teamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))
                .ToList();
            return View(teamSeasonIndexViewModel);
        }

        // GET: TeamSeasons/Details/5
        /// <summary>
        /// Renders a view of a selected team season.
        /// </summary>
        /// <param name="id">The Id of the selected team season.</param>
        /// <returns>The rendered view of the selected team season.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var teamSeason = await teamSeasonRepository.GetTeamSeasonAsync(id.Value);
            if (teamSeason is null)
            {
                return NotFound();
            }
            teamSeasonDetailsViewModel.TeamSeason = teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason);

            var teamId = teamSeason.TeamId;
            var seasonId = teamSeason.SeasonId;
            teamSeasonDetailsViewModel.TeamSeasonScheduleProfile =
                await teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId);
            teamSeasonDetailsViewModel.TeamSeasonScheduleTotals =
                await teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonId);
            teamSeasonDetailsViewModel.TeamSeasonScheduleAverages =
                await teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonId);

            return View(teamSeasonDetailsViewModel);
        }

        // TeamSeasons/RunWeeklyUpdate
        /// <summary>
        /// Runs a weekly update of the TeamSeasons list.
        /// </summary>
        /// <returns>The rendered view of the team seasons index.</returns>
        [HttpGet]
        public async Task<IActionResult> RunWeeklyUpdate()
        {
            // TODO - 2026-05-15 - Remove the following hack when multiple leagues are supported.
            var dbContext = new ProFootballDbContext();
            var leagueRepository = new LeagueRepository(dbContext);
            var leagueName = "APFA";
            var leagueId = (await leagueRepository.GetLeagueByShortNameAsync(leagueName)).Id;

            var selectedSeasonYear = HttpContext.Session.GetObject<int>("SelectedSeasonYear");
            await weeklyUpdateService.RunWeeklyUpdate(leagueId, selectedSeasonYear);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected season year.
        /// </summary>
        /// <param name="seasonYear">The season year to which the selected season year will be set.</param>
        /// <returns>The rendered view of the team seasons index.</returns>
        public IActionResult SetSelectedSeasonYear(int? seasonYear)
        {
            if (seasonYear is null)
            {
                return BadRequest();
            }

            HttpContext.Session.SetObject("SelectedSeasonYear", seasonYear);

            return RedirectToAction(nameof(Index));
        }
    }
}
