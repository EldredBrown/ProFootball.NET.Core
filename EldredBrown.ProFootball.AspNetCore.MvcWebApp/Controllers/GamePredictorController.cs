using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public enum SessionKey
    {
        Seasons,
        GuestSeasonYear,
        GuestTeamSeasons,
        GuestName,
        HostSeasonYear,
        HostTeamSeasons,
        HostName,
    }

    /// <summary>
    /// Provides control of the flow of execution for the game predictor.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GamePredictorController"/> class.
    /// </remarks>
    /// <param name="seasonRepository">
    /// The <see cref="IAssociationRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="teamSeasonRepository">
    /// The <see cref="ITeamSeasonRepository"/> by which team season data will be accessed.
    /// </param>
    /// <param name="gamePredictorService">
    /// The <see cref="IGamePredictorService"/> by which a game prediction will be calculated.
    /// </param>
    public class GamePredictorController(
        GamePrediction prediction,
        ISeasonRepository seasonRepository,
        ITeamSeasonRepository teamSeasonRepository,
        IGamePredictorService gamePredictorService
    ) : Controller
    {
        // GET: GamePredictor/PredictGame
        /// <summary>
        /// Renders a view of the Game Predictor form.
        /// </summary>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        [HttpGet]
        public async Task<IActionResult> PredictGame()
        {
            var seasons = (await seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Year).ToList();
            HttpContext.Session.SetObject(nameof(SessionKey.Seasons), seasons);

            await SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(seasons, SessionKey.GuestSeasonYear,
                SessionKey.GuestTeamSeasons, SessionKey.GuestName);
            await SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(seasons, SessionKey.HostSeasonYear,
                SessionKey.HostTeamSeasons, SessionKey.HostName);

            return View(prediction);
        }

        // POST: GamePredictor/PredictGame
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the Game Predictor form.
        /// </summary>
        /// <param name="prediction">A <see cref="IGamePredictionViewModel"/> object representing the game matchup.</param>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        [HttpPost]
        public async Task<IActionResult> PredictGame(
            [Bind("GuestSeasonYear,GuestName,GuestScore,HostSeasonYear,HostName,HostScore")] GamePrediction prediction
        )
        {
            var seasons = HttpContext.Session.GetObject<IEnumerable<Season>>(nameof(SessionKey.Seasons));

            var guestSeasonYear = prediction.GuestSeasonYear;
            HttpContext.Session.SetObject(nameof(SessionKey.GuestSeasonYear), guestSeasonYear);
            ViewBag.GuestSeasons = new SelectList(seasons, "Year", "Year", guestSeasonYear);

            var guestTeamSeasons = HttpContext.Session.GetObject<IEnumerable<TeamSeason>>(nameof(SessionKey.GuestTeamSeasons));
            var guestTeamSeason = guestTeamSeasons.FirstOrDefault(ts => ts.TeamIdNavigation.Name == prediction.GuestName);
            if (guestTeamSeason is null)
            {
                ViewBag.Guests = new SelectList(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList(),
                    prediction.GuestName);
            }
            else
            {
                ViewBag.Guests = new SelectList(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList(),
                    guestTeamSeason.TeamIdNavigation.Name);
            }

            var hostSeasonYear = prediction.HostSeasonYear;
            HttpContext.Session.SetObject(nameof(SessionKey.HostSeasonYear), hostSeasonYear);
            ViewBag.HostSeasons = new SelectList(seasons, "Year", "Year", hostSeasonYear);

            var hostTeamSeasons = HttpContext.Session.GetObject<IEnumerable<TeamSeason>>(nameof(SessionKey.HostTeamSeasons));
            var hostTeamSeason = hostTeamSeasons.FirstOrDefault(ts => ts.TeamIdNavigation.Name == prediction.HostName);
            if (hostTeamSeason is null)
            {
                ViewBag.Hosts = new SelectList(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList(),
                    prediction.HostName);
            }
            else
            {
                ViewBag.Hosts = new SelectList(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList(),
                    hostTeamSeason.TeamIdNavigation.Name);
            }

            var gameScorePrediction = new GameScorePrediction();
            try
            {
                gameScorePrediction = gamePredictorService.PredictGameScore(guestTeamSeason, hostTeamSeason);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "A prediction could not be calculated for the selected teams.");
                return View(prediction);
            }
            prediction.GuestScore = gameScorePrediction.GuestScore.Value;
            prediction.HostScore = gameScorePrediction.HostScore.Value;

            return View(prediction);
        }

        /// <summary>
        /// Applies a filter to listed guest or host data.
        /// </summary>
        /// <param name="guestSeasonYear">The season for which possible guests will be shown.</param>
        /// <param name="hostSeasonYear">The season for which possible hosts will be shown.</param>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        public IActionResult ApplyFilter(int? guestSeasonYear, string guestName, int? hostSeasonYear, string hostName)
        {
            if (guestSeasonYear.HasValue)
            {
                HttpContext.Session.SetObject("GuestSeasonYear", guestSeasonYear);
            }

            if (!guestName.IsNullOrEmpty())
            {
                HttpContext.Session.SetObject("GuestName", guestName);
            }

            if (hostSeasonYear.HasValue)
            {
                HttpContext.Session.SetObject("HostSeasonYear", hostSeasonYear);
            }

            if (!hostName.IsNullOrEmpty())
            {
                HttpContext.Session.SetObject("HostName", hostName);
            }

            return RedirectToAction(nameof(PredictGame));
        }

        /// <summary>
        /// Sets the selected name for the specified team.
        /// </summary>
        /// <param name="teamName">The name to which the selected season team will be set.</param>
        /// <returns>The rendered view of the team seasons index.</returns>
        public IActionResult SetTeamSeasonName(string sessionKey, string teamName)
        {
            if (teamName.IsNullOrEmpty())
            {
                return BadRequest();
            }
            HttpContext.Session.SetObject(sessionKey, teamName);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected season year for the specified team.
        /// </summary>
        /// <param name="seasonYear">The season year to which the selected season year will be set.</param>
        /// <returns>The rendered view of the team seasons index.</returns>
        public IActionResult SetTeamSeasonYear(string sessionKey, int? seasonYear)
        {
            if (seasonYear is null)
            {
                return BadRequest();
            }
            HttpContext.Session.SetObject(sessionKey, seasonYear);
            return RedirectToAction(nameof(Index));
        }

        private void SelectTeamName(SessionKey teamNameSessionKey, IEnumerable<TeamSeason> teamSeasons)
        {
            var teamName = HttpContext.Session.GetObject<string>(teamNameSessionKey.ToString());
            if (teamName.IsNullOrEmpty())
            {
                teamName = teamSeasons.FirstOrDefault()?.TeamIdNavigation.Name;
                SetTeamSeasonName(teamNameSessionKey.ToString(), teamName);
            }
            var teamsSelectList = new SelectList(teamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList(), teamName);
            switch (teamNameSessionKey)
            {
                case SessionKey.GuestName:
                    ViewBag.Guests = teamsSelectList;
                    prediction.GuestName = teamName;
                    break;
                case SessionKey.HostName:
                    ViewBag.Hosts = teamsSelectList;
                    prediction.HostName = teamName;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected session key: {teamNameSessionKey}");
            }
        }

        private async Task SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(IEnumerable<Season> seasons,
            SessionKey teamSeasonYearSessionKey, SessionKey teamSeasonsSessionKey, SessionKey teamNameSessionKey)
        {
            int? teamSeasonYear = SelectTeamSeasonYear(seasons, teamSeasonYearSessionKey);
            //var teamSeasonId = seasons.FirstOrDefault(s => s.Year == teamSeasonYear)?.Id;
            IEnumerable<TeamSeason> teamSeasons = await GetTeamSeasons(teamSeasonsSessionKey, teamSeasonYear);
            SelectTeamName(teamNameSessionKey, teamSeasons);
        }

        private int? SelectTeamSeasonYear(IEnumerable<Season> seasons, SessionKey teamSeasonYearSessionKey)
        {
            var teamSeasonYear = HttpContext.Session.GetObject<int?>(teamSeasonYearSessionKey.ToString());
            var teamSeason = seasons.FirstOrDefault(s => s.Year == teamSeasonYear);
            if (teamSeason is null)
            {
                teamSeason = seasons.First();
                SetTeamSeasonYear(teamSeasonYearSessionKey.ToString(), teamSeason.Year);
            }
            var teamSeasonSelectList = new SelectList(seasons, "Year", "Year", teamSeason.Year);
            switch (teamSeasonYearSessionKey)
            {
                case SessionKey.GuestSeasonYear:
                    ViewBag.GuestSeasons = teamSeasonSelectList;
                    prediction.GuestSeasonYear = teamSeason.Year;
                    break;
                case SessionKey.HostSeasonYear:
                    ViewBag.HostSeasons = teamSeasonSelectList;
                    prediction.HostSeasonYear = teamSeason.Year;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected session key: {teamSeasonYearSessionKey}");
            }

            return teamSeason.Year;
        }

        private async Task<IEnumerable<TeamSeason>> GetTeamSeasons(SessionKey teamSeasonsSessionKey, int? teamSeasonId)
        {
            var teamSeasons = await teamSeasonRepository.GetTeamSeasonsBySeasonAsync(teamSeasonId.Value);
            HttpContext.Session.SetObject(teamSeasonsSessionKey.ToString(), teamSeasons);
            return teamSeasons;
        }
    }
}
