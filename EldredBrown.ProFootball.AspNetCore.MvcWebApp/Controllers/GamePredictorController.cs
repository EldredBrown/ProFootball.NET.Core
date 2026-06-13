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
        GuestSeasonId,
        GuestTeamSeasons,
        GuestName,
        HostSeasonId,
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
    /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="teamSeasonRepository">
    /// The <see cref="ITeamSeasonRepository"/> by which team season data will be accessed.
    /// </param>
    /// <param name="gamePredictorService">
    /// The <see cref="IGamePredictorService"/> by which a game prediction will be calculated.
    /// </param>
    public class GamePredictorController(
        IGamePrediction prediction,
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
            var seasons = (await seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Id).ToList();
            HttpContext.Session.SetObject("Seasons", seasons);

            await SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(seasons, SessionKey.GuestSeasonId,
                SessionKey.GuestTeamSeasons, SessionKey.GuestName);
            await SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(seasons, SessionKey.HostSeasonId,
                SessionKey.HostTeamSeasons, SessionKey.HostName);

            return View(prediction);
        }

        // POST: GamePredictor/PredictGame
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the Game Predictor form.
        /// </summary>
        /// <param name="prediction">A <see cref="GamePrediction"/> object representing the game matchup.</param>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        [HttpPost]
        public async Task<IActionResult> PredictGame([Bind("GuestSeasonId,GuestName,GuestScore,HostSeasonId,HostName,HostScore")] GamePrediction prediction)
        {
            var seasons = HttpContext.Session.GetObject<IEnumerable<Season>>(nameof(SessionKey.Seasons));

            var guestSeasonId = prediction.GuestSeasonId;
            HttpContext.Session.SetObject(nameof(SessionKey.GuestSeasonId), guestSeasonId);
            ViewBag.GuestSeasons = new SelectList(seasons, "Id", "Id", guestSeasonId);

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

            var hostSeasonId = prediction.HostSeasonId;
            HttpContext.Session.SetObject("HostSeasonId", hostSeasonId);
            ViewBag.HostSeasons = new SelectList(seasons, "Id", "Id", hostSeasonId);

            var hostTeamSeasons = HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
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
        /// <param name="guestSeasonId">The season for which possible guests will be shown.</param>
        /// <param name="hostSeasonId">The season for which possible hosts will be shown.</param>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        public IActionResult ApplyFilter(int? guestSeasonId, string guestName, int? hostSeasonId, string hostName)
        {
            if (guestSeasonId.HasValue)
            {
                HttpContext.Session.SetObject("GuestSeasonId", guestSeasonId);
            }

            if (!guestName.IsNullOrEmpty())
            {
                HttpContext.Session.SetObject("GuestName", guestName);
            }

            if (hostSeasonId.HasValue)
            {
                HttpContext.Session.SetObject("HostSeasonId", hostSeasonId);
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

        private async Task SelectTeamSeasonYearGetTeamSeasonsAndSelectTeamName(IEnumerable<Season> seasons,
            SessionKey teamSeasonYearSessionKey, SessionKey teamSeasonsSessionKey, SessionKey teamNameSessionKey)
        {
            int? teamSeasonYear = SelectTeamSeasonYear(seasons, teamSeasonYearSessionKey);
            IEnumerable<TeamSeason> teamSeasons = await GetTeamSeasons(teamSeasonsSessionKey, teamSeasonYear);
            SelectTeamName(teamNameSessionKey, teamSeasons);
        }

        private int? SelectTeamSeasonYear(IEnumerable<Season> seasons, SessionKey teamSeasonYearSessionKey)
        {
            var teamSeasonYear = HttpContext.Session.GetObject<int?>(teamSeasonYearSessionKey.ToString());
            if (teamSeasonYear is null)
            {
                teamSeasonYear = seasons.First().Id;
                SetTeamSeasonYear(teamSeasonYearSessionKey.ToString(), teamSeasonYear.Value);
            }
            var teamSeasonSelectList = new SelectList(seasons, "Id", "Id", teamSeasonYear);
            switch (teamSeasonYearSessionKey)
            {
                case SessionKey.GuestSeasonId:
                    ViewBag.GuestSeasons = teamSeasonSelectList;
                    prediction.GuestSeasonId = teamSeasonYear.Value;
                    break;
                case SessionKey.HostSeasonId:
                    ViewBag.HostSeasons = teamSeasonSelectList;
                    prediction.HostSeasonId = teamSeasonYear.Value;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected session key: {teamSeasonYearSessionKey}");
            }

            return teamSeasonYear;
        }

        private async Task<IEnumerable<TeamSeason>> GetTeamSeasons(SessionKey teamSeasonsSessionKey, int? teamSeasonYear)
        {
            var teamSeasons = await teamSeasonRepository.GetTeamSeasonsBySeasonAsync(teamSeasonYear.Value);
            HttpContext.Session.SetObject(teamSeasonsSessionKey.ToString(), teamSeasons);
            return teamSeasons;
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
    }
}
