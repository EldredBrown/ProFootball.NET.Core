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
        ILeagueRepository leagueRepository,
        ISeasonRankingsRepository seasonRankingsRepository
        ) : Controller
    {
        private const string _defaultLeagueName = "NFL";

        // GET: SeasonRankings
        /// <summary>
        /// Renders a view of the SeasonRankings list.
        /// </summary>
        /// <returns>The rendered view of the SeasonRankings list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? selectedSeasonYear = await SelectSeason();
            League selectedLeague = await SelectLeague();
            SeasonRankingType? selectedRankingType = SelectRankingType();
            await GetSelectedRankings(selectedSeasonYear, selectedRankingType.Value);
            return View(seasonRankingsIndexViewModel);
        }

        /// <summary>
        /// Sets the selected season Id.
        /// </summary>
        /// <param name="leagueName">The name of the selected league.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedLeagueName(string leagueShortName)
        {
            if (leagueShortName.IsNullOrEmpty())
            {
                return BadRequest();
            }
            HttpContext.Session.SetObject("SelectedLeagueName", leagueShortName);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected ranking type.
        /// </summary>
        /// <param name="rankingType">The selected league.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedRankingType(SeasonRankingType? rankingType)
        {
            HttpContext.Session.SetObject("SelectedRankingType", rankingType);
            return RedirectToAction(nameof(Index));
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

        private async Task GetSelectedRankings(int? selectedSeasonYear, SeasonRankingType selectedRankingType)
        {
            switch (selectedRankingType)
            {
                case SeasonRankingType.Offensive:
                    seasonRankingsIndexViewModel.SeasonRankings =
                        await seasonRankingsRepository.GetOffensiveRankingsForSeasonAsync(selectedSeasonYear.Value);
                    break;
                case SeasonRankingType.Defensive:
                    seasonRankingsIndexViewModel.SeasonRankings =
                        await seasonRankingsRepository.GetDefensiveRankingsForSeasonAsync(selectedSeasonYear.Value);
                    break;
                case SeasonRankingType.Total:
                    seasonRankingsIndexViewModel.SeasonRankings =
                        await seasonRankingsRepository.GetTotalRankingsForSeasonAsync(selectedSeasonYear.Value);
                    break;
                case SeasonRankingType.None:
                    seasonRankingsIndexViewModel.SeasonRankings = [];
                    break;
            }
        }

        private async Task<League> SelectLeague()
        {
            var leagues = (await leagueRepository.GetLeaguesAsync()).OrderBy(l => l.Id);
            HttpContext.Session.SetObject("Leagues", leagues);

            var selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            if (selectedLeagueName.IsNullOrEmpty())
            {
                selectedLeagueName = _defaultLeagueName;
                SetSelectedLeagueName(_defaultLeagueName);
            }
            seasonRankingsIndexViewModel.Leagues = new SelectList(leagues, "ShortName", "ShortName", selectedLeagueName);
            seasonRankingsIndexViewModel.SelectedLeague = selectedLeagueName;
            return await leagueRepository.GetLeagueByShortNameAsync(selectedLeagueName);
        }

        private SeasonRankingType SelectRankingType()
        {
            var selectedRankingType = HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType");
            if (selectedRankingType is null)
            {
                selectedRankingType = SeasonRankingType.None;
                SetSelectedRankingType(selectedRankingType.Value);
            }
            seasonRankingsIndexViewModel.RankingTypes = new SelectList(
                Enum.GetValues<SeasonRankingType>()
                    .Select(e => new { Value = (int)e, Text = e.ToString() }),
                "Value",
                "Text",
                selectedRankingType.Value
            );
            seasonRankingsIndexViewModel.SelectedRankingType = selectedRankingType.Value;
            return selectedRankingType.Value;
        }

        private async Task<int?> SelectSeason()
        {
            var seasons = (await seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Id);
            HttpContext.Session.SetObject("Seasons", seasons);

            var selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            if (selectedSeasonYear is null)
            {
                selectedSeasonYear = seasons.First().Id;
                SetSelectedSeasonYear(selectedSeasonYear);
            }
            seasonRankingsIndexViewModel.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);
            seasonRankingsIndexViewModel.SelectedSeasonYear = selectedSeasonYear;
            return selectedSeasonYear;
        }
    }
}
