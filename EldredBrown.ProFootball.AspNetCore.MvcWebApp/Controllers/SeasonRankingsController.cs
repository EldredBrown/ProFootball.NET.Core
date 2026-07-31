using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public enum SeasonRankingType
    {
        None,
        Offensive,
        Defensive,
        Total
    }

    /// <summary>
    /// Provides control of the flow of execution for views of season rankings data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonRankingsController"/> class.
    /// </remarks>
    /// <param name="seasonRankingsIndexViewModel">
    /// The <see cref="ISeasonRankingsIndexViewModel"/> by which data will be modeled for the season rankings
    /// index view.
    /// </param>
    /// <param name="seasonRepository">
    /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="seasonRankingsRepository">
    /// The <see cref="ISeasonRankingsRepository"/> by which season rankings data will be accessed.
    /// </param>
    public class SeasonRankingsController(
        ISeasonRankingsIndexViewModel seasonRankingsIndexViewModel,
        ISeasonRepository seasonRepository,
        IAssociationRepository associationRepository,
        ISeasonRankingsRepository seasonRankingsRepository
    ) : Controller
    {
        internal readonly ISeasonRankingsIndexViewModel _seasonRankingsIndexViewModel = seasonRankingsIndexViewModel;
        internal readonly ISeasonRepository _seasonRepository = seasonRepository;
        internal readonly IAssociationRepository _associationRepository = associationRepository;
        internal readonly ISeasonRankingsRepository _seasonRankingsRepository = seasonRankingsRepository;

        private int? _selectedSeasonYear = null;
        private string _selectedLeagueName = null;
        private SeasonRankingType? _selectedRankingType = SeasonRankingType.None;

        // GET: SeasonRankings
        /// <summary>
        /// Renders a view of the SeasonRankings list.
        /// </summary>
        /// <returns>The rendered view of the SeasonRankings list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel();
            await LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel();
            LoadRankingTypesAndSelectedRankingTypeIntoIndexViewModel();
            await GetSelectedRankings();

            return View(_seasonRankingsIndexViewModel);
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
        /// Sets the selected ranking type.
        /// </summary>
        /// <param name="rankingType">The selected league.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedRankingType(SeasonRankingType rankingType)
        {
            _selectedRankingType = rankingType;
            HttpContext.Session.SetObject("SelectedRankingType", rankingType);

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

        private async Task GetSelectedRankings()
        {
            var selectedLeague = await _associationRepository.GetAssociationByShortNameAsync(_selectedLeagueName);

            switch (_selectedRankingType)
            {
                case SeasonRankingType.Offensive:
                    _seasonRankingsIndexViewModel.SeasonRankings =
                        await _seasonRankingsRepository.GetOffensiveRankingsAsync(_selectedSeasonYear.Value, selectedLeague.Id);
                    break;
                case SeasonRankingType.Defensive:
                    _seasonRankingsIndexViewModel.SeasonRankings =
                        await _seasonRankingsRepository.GetDefensiveRankingsAsync(_selectedSeasonYear.Value, selectedLeague.Id);
                    break;
                case SeasonRankingType.Total:
                    _seasonRankingsIndexViewModel.SeasonRankings =
                        await _seasonRankingsRepository.GetTotalRankingsAsync(_selectedSeasonYear.Value, selectedLeague.Id);
                    break;
                case SeasonRankingType.None:
                    _seasonRankingsIndexViewModel.SeasonRankings = [];
                    break;
            }
        }

        private async Task LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel()
        {
            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");

            var leagues = await GetLeagues();
            if (_selectedLeagueName.IsNullOrEmpty())
            {
                SetSelectedLeagueName(leagues.First().ShortName);
            }
            _seasonRankingsIndexViewModel.Leagues = new SelectList(leagues, "ShortName", "ShortName", _selectedLeagueName);
            _seasonRankingsIndexViewModel.SelectedLeagueName = _selectedLeagueName;
        }

        private SeasonRankingType LoadRankingTypesAndSelectedRankingTypeIntoIndexViewModel()
        {
            _selectedRankingType = HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType");

            if (_selectedRankingType is null)
            {
                _selectedRankingType = SeasonRankingType.None;
                SetSelectedRankingType(_selectedRankingType.Value);
            }

            _seasonRankingsIndexViewModel.RankingTypes = new SelectList(
                Enum.GetValues<SeasonRankingType>()
                    .Select(e => new { Value = (int)e, Text = e.ToString() }),
                "Value",
                "Text",
                _selectedRankingType.Value
            );

            _seasonRankingsIndexViewModel.SelectedRankingType = _selectedRankingType.Value;
            return _selectedRankingType.Value;
        }

        private async Task LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel()
        {
            _selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");

            var seasons = await GetSeasons();
            if (_selectedSeasonYear is null)
            {
                SetSelectedSeasonYear(seasons.First().Year);
            }
            _seasonRankingsIndexViewModel.Seasons = new SelectList(seasons, "Year", "Year", _selectedSeasonYear);
            _seasonRankingsIndexViewModel.SelectedSeasonYear = _selectedSeasonYear;
        }
    }
}
