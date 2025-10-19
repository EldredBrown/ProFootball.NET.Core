using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of season standings data.
    /// </summary>
    public class SeasonStandingsController : Controller
    {
        public static int SelectedSeasonYear = 1920;
        public static bool GroupByDivision = false;

        private readonly ISeasonStandingsIndexViewModel _seasonStandingsIndexViewModel;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISeasonStandingsRepository _seasonStandingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonStandingsController"/> class.
        /// </summary>
        /// <param name="seasonStandingsIndexViewModel">
        /// The <see cref="ISeasonStandingsIndexViewModel"/> by which data will be modeled for the season standings
        /// index view.
        /// </param>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="seasonStandingsRepository">
        /// The <see cref="ISeasonStandingsRepository"/> by which season standings data will be accessed.
        /// </param>
        public SeasonStandingsController(
            ISeasonStandingsIndexViewModel seasonStandingsIndexViewModel,
            ISeasonRepository seasonRepository,
            ISeasonStandingsRepository seasonStandingsRepository)
        {
            _seasonStandingsIndexViewModel = seasonStandingsIndexViewModel;
            _seasonRepository = seasonRepository;
            _seasonStandingsRepository = seasonStandingsRepository;
        }

        // GET: SeasonStandings
        /// <summary>
        /// Renders a view of the SeasonStandings list.
        /// </summary>
        /// <returns>The rendered view of the SeasonStandings list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var seasons = (await _seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Year);

            _seasonStandingsIndexViewModel.Seasons = new SelectList(seasons, "Year", "Year", SelectedSeasonYear);
            _seasonStandingsIndexViewModel.SelectedSeasonYear = SelectedSeasonYear;
            _seasonStandingsIndexViewModel.SeasonStandings =
                await _seasonStandingsRepository.GetSeasonStandingsAsync(SelectedSeasonYear);

            return View(_seasonStandingsIndexViewModel);
        }

        /// <summary>
        /// Sets the selected season ID.
        /// </summary>
        /// <param name="seasonYear">The ID of the selected season.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedSeasonYear(int? seasonYear)
        {
            if (seasonYear is null)
            {
                return BadRequest();
            }

            SelectedSeasonYear = seasonYear.Value;

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the groupByDivision flag.
        /// </summary>
        /// <param name="groupByDivision">Indicates whether the groupByDivision flag should be set to true or false.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetGroupByDivision(bool? groupByDivision)
        {
            if (groupByDivision.HasValue)
            {
                GroupByDivision = groupByDivision.Value;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
