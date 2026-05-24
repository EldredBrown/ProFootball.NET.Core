using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public class TeamSeasonController : Controller
    {
        public static int SelectedSeasonYear = 1920;

        private readonly ITeamSeasonIndexViewModel _teamSeasonIndexViewModel;
        private readonly ITeamSeasonDetailsViewModel _teamSeasonDetailsViewModel;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;
        private readonly IWeeklyUpdateService _weeklyUpdateService;

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
        public TeamSeasonController(
            ITeamSeasonIndexViewModel teamSeasonIndexViewModel,
            ITeamSeasonDetailsViewModel teamSeasonDetailsViewModel,
            ISeasonRepository seasonRepository,
            ITeamSeasonRepository teamSeasonRepository,
            ITeamSeasonScheduleRepository teamSeasonScheduleRepository,
            IWeeklyUpdateService weeklyUpdateService)
        {
            _teamSeasonIndexViewModel = teamSeasonIndexViewModel;
            _teamSeasonDetailsViewModel = teamSeasonDetailsViewModel;
            _seasonRepository = seasonRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _teamSeasonScheduleRepository = teamSeasonScheduleRepository;
            _weeklyUpdateService = weeklyUpdateService;
        }

        // GET: TeamSeasons
        /// <summary>
        /// Renders a view of the team seasons index.
        /// </summary>
        /// <returns>The rendered view of the team seasons index.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var seasons = await _seasonRepository.GetSeasonsAsync();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            _teamSeasonIndexViewModel.Seasons = new SelectList(orderedSeasons, "Year", "Year", SelectedSeasonYear);
            _teamSeasonIndexViewModel.SelectedSeasonYear = SelectedSeasonYear;
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(SelectedSeasonYear);
            _teamSeasonIndexViewModel.TeamSeasons = teamSeasons;

            return View(_teamSeasonIndexViewModel);
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

            var teamSeason = await _teamSeasonRepository.GetTeamSeasonAsync(id.Value);
            if (teamSeason is null)
            {
                return NotFound();
            }
            _teamSeasonDetailsViewModel.TeamSeason = teamSeason;

            var teamName = teamSeason.TeamName;
            var seasonYear = teamSeason.SeasonYear;
            _teamSeasonDetailsViewModel.TeamSeasonScheduleProfile =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear);
            _teamSeasonDetailsViewModel.TeamSeasonScheduleTotals =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear);
            _teamSeasonDetailsViewModel.TeamSeasonScheduleAverages =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear);

            return View(_teamSeasonDetailsViewModel);
        }

        // TeamSeasons/RunWeeklyUpdate
        /// <summary>
        /// Runs a weekly update of the TeamSeasons list.
        /// </summary>
        /// <returns>The rendered view of the team seasons index.</returns>
        [HttpGet]
        public async Task<IActionResult> RunWeeklyUpdate()
        {
            // TODO - 2026-05-15 - Remove hardcoded league name when multiple leagues are supported.
            var leagueName = "NFL";

            await _weeklyUpdateService.RunWeeklyUpdate(leagueName, SelectedSeasonYear);

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

            SelectedSeasonYear = seasonYear.Value;

            return RedirectToAction(nameof(Index));
        }
    }
}
