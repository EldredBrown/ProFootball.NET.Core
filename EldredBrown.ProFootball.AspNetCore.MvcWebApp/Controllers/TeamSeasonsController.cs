using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public class TeamSeasonsController : Controller
    {
        public static int SelectedSeasonYear = 1920;

        private readonly ITeamSeasonsIndexViewModel _teamSeasonsIndexViewModel;
        private readonly ITeamSeasonsDetailsViewModel _teamSeasonsDetailsViewModel;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IWeeklyUpdateService _weeklyUpdateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonsController"/> class.
        /// </summary>
        /// <param name="teamSeasonsIndexViewModel">
        /// The <see cref="ITeamSeasonsIndexViewModel"/> that will provide data to the TeamSeasons index view.
        /// </param>
        /// <param name="teamSeasonsDetailsViewModel">
        /// The <see cref="ITeamSeasonsDetailsViewModel"/> that will provide data to the TeamSeasons details view.
        /// </param>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="teamSeasonRepository">
        /// The <see cref="ITeamSeasonRepository"/> by which team season data will be accessed.
        /// </param>
        /// <param name="teamSeasonScheduleProfileRepository">
        /// The <see cref="ITeamSeasonScheduleProfileRepository"/> by which team season schedule profile data will be
        /// accessed.
        /// </param>
        /// <param name="teamSeasonScheduleTotalsRepository">
        /// The <see cref="ITeamSeasonScheduleTotalsRepository"/> by which team season schedule totals data will be
        /// accessed.
        /// </param>
        /// <param name="teamSeasonScheduleAveragesRepository">
        /// The <see cref="ITeamSeasonScheduleAveragesRepository"/> by which team season schedule averages data will be
        /// accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        /// <param name="weeklyUpdateService">
        /// The <see cref="IWeeklyUpdateService"/> that will run weekly updates of the data store.
        /// </param>
        public TeamSeasonsController(
            ITeamSeasonsIndexViewModel teamSeasonsIndexViewModel,
            ITeamSeasonsDetailsViewModel teamSeasonsDetailsViewModel,
            ISeasonRepository seasonRepository,
            ITeamSeasonRepository teamSeasonRepository,
            ITeamSeasonScheduleRepository teamSeasonScheduleRepository,
            ISharedRepository sharedRepository,
            IWeeklyUpdateService weeklyUpdateService)
        {
            _teamSeasonsIndexViewModel = teamSeasonsIndexViewModel;
            _teamSeasonsDetailsViewModel = teamSeasonsDetailsViewModel;
            _seasonRepository = seasonRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _teamSeasonScheduleRepository = teamSeasonScheduleRepository;
            _sharedRepository = sharedRepository;
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

            _teamSeasonsIndexViewModel.Seasons = new SelectList(orderedSeasons, "Year", "Year", SelectedSeasonYear);
            _teamSeasonsIndexViewModel.SelectedSeasonYear = SelectedSeasonYear;
            _teamSeasonsIndexViewModel.TeamSeasons =
                await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(SelectedSeasonYear);

            return View(_teamSeasonsIndexViewModel);
        }

        // GET: TeamSeasons/Details/5
        /// <summary>
        /// Renders a view of a selected team season.
        /// </summary>
        /// <param name="id">The ID of the selected team season.</param>
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
            _teamSeasonsDetailsViewModel.TeamSeason = teamSeason;

            var teamName = teamSeason.TeamName;
            var seasonYear = teamSeason.SeasonYear;
            _teamSeasonsDetailsViewModel.TeamSeasonScheduleProfile =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear);
            _teamSeasonsDetailsViewModel.TeamSeasonScheduleTotals =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear);
            _teamSeasonsDetailsViewModel.TeamSeasonScheduleAverages =
                await _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear);

            return View(_teamSeasonsDetailsViewModel);
        }

        // TeamSeasons/RunWeeklyUpdate
        /// <summary>
        /// Runs a weekly update of the TeamSeasons list.
        /// </summary>
        /// <returns>The rendered view of the team seasons index.</returns>
        [HttpGet]
        public async Task<IActionResult> RunWeeklyUpdate()
        {
            await _weeklyUpdateService.RunWeeklyUpdate(SelectedSeasonYear);

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
