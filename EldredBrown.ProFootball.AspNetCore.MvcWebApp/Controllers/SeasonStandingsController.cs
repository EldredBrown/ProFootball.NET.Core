using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.Net.Data.Models;
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
        IAssociationRepository associationRepository,
        ISeasonStandingsRepository seasonStandingsRepository
    ) : Controller
    {
        internal readonly ISeasonStandingsIndexViewModel _seasonStandingsIndexViewModel = seasonStandingsIndexViewModel;
        internal readonly ISeasonRepository _seasonRepository = seasonRepository;
        internal readonly IAssociationRepository _associationRepository = associationRepository;
        internal readonly ISeasonStandingsRepository _seasonStandingsRepository = seasonStandingsRepository;

        private int? _selectedSeasonYear = null;
        private string _selectedLeagueName = null;

        // GET: SeasonStandings
        /// <summary>
        /// Renders a view of the SeasonStandings list.
        /// </summary>
        /// <returns>The rendered view of the SeasonStandings list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel();
            await LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel();

            var selectedLeague = await _associationRepository.GetAssociationByShortNameAsync(_selectedLeagueName);
            _seasonStandingsIndexViewModel.SeasonStandings =
                await _seasonStandingsRepository.GetSeasonStandingsAsync(_selectedSeasonYear.Value, selectedLeague.Id);

            return View(_seasonStandingsIndexViewModel);
        }

        /// <summary>
        /// Sets the selected season year.
        /// </summary>
        /// <param name="seasonYear">The year of the selected season.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedSeasonYear(int? seasonYear)
        {
            if (seasonYear is null)
            {
                return BadRequest();
            }

            _selectedSeasonYear = seasonYear.Value;
            HttpContext.Session.SetObject("SelectedSeasonYear", seasonYear.Value);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected league Id.
        /// </summary>
        /// <param name="seasonYear">The year of the selected season.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedLeagueName(string leagueName)
        {
            if (leagueName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            _selectedLeagueName = leagueName;
            HttpContext.Session.SetObject("SelectedLeagueName", leagueName);

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

        private async Task<IEnumerable<Association>> GetLeagues()
        {
            return [.. (await _associationRepository.GetAssociationsAsync())
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= _selectedSeasonYear
                    && (l.LastSeasonYearNavigation is null || _selectedSeasonYear <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)];
        }

        private async Task<IEnumerable<Season>> GetSeasons()
        {
            return [.. (await _seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Year)];
        }

        private async Task LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel()
        {
            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");

            var leagues = await GetLeagues();
            if (_selectedLeagueName.IsNullOrEmpty())
            {
                SetSelectedLeagueName(leagues.First().ShortName);
            }
            _seasonStandingsIndexViewModel.Leagues = new SelectList(leagues, "ShortName", "ShortName", _selectedLeagueName);
            _seasonStandingsIndexViewModel.SelectedLeagueName = _selectedLeagueName;
        }

        private async Task LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel()
        {
            _selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");

            var seasons = await GetSeasons();
            if (_selectedSeasonYear is null)
            {
                SetSelectedSeasonYear(seasons.First().Year);
            }
            _seasonStandingsIndexViewModel.Seasons = new SelectList(seasons, "Year", "Year", _selectedSeasonYear);
            _seasonStandingsIndexViewModel.SelectedSeasonYear = _selectedSeasonYear;
        }
    }
}
