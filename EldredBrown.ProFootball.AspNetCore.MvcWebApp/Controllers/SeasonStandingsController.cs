using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of season standings data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonStandingsController"/> class.
    /// </remarks>
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
    public class SeasonStandingsController(
        ISeasonStandingsIndexViewModel seasonStandingsIndexViewModel,
        ISeasonRepository seasonRepository,
        ISeasonStandingsRepository seasonStandingsRepository
        ) : Controller
    {
        // GET: SeasonStandings
        /// <summary>
        /// Renders a view of the SeasonStandings list.
        /// </summary>
        /// <returns>The rendered view of the SeasonStandings list.</returns>
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
            seasonStandingsIndexViewModel.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);
            seasonStandingsIndexViewModel.SelectedSeasonYear = selectedSeasonYear;
            seasonStandingsIndexViewModel.SeasonStandings =
                await seasonStandingsRepository.GetSeasonStandingsAsync(selectedSeasonYear.Value);

            return View(seasonStandingsIndexViewModel);
        }

        /// <summary>
        /// Sets the selected season Id.
        /// </summary>
        /// <param name="seasonYear">The Id of the selected season.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedSeasonYear(int? seasonYear)
        {
            if (seasonYear is null)
            {
                return BadRequest();
            }

            HttpContext.Session.SetObject("SelectedSeasonYear", seasonYear);

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
                HttpContext.Session.SetObject("GroupByDivision", groupByDivision.Value);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
