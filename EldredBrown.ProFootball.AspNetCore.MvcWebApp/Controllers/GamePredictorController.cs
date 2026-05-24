using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for the game predictor.
    /// </summary>
    public class GamePredictorController : Controller
    {
        public static int GuestSeasonYear = 1920;
        public static int HostSeasonYear = 1920;

        private readonly ISeasonRepository _seasonRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly IGamePredictorService _gamePredictorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePredictorController"/> class.
        /// </summary>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="teamSeasonRepository">
        /// The <see cref="ITeamSeasonRepository"/> by which team season data will be accessed.
        /// </param>
        /// <param name="gamePredictorService">
        /// The <see cref="IGamePredictorService"/> by which a game prediction will be calculated.
        /// </param>
        public GamePredictorController(
            ISeasonRepository seasonRepository, ITeamSeasonRepository teamSeasonRepository,
            IGamePredictorService gamePredictorService)
        {
            _seasonRepository = seasonRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _gamePredictorService = gamePredictorService;
        }

        // GET: GamePredictor/PredictGame
        /// <summary>
        /// Renders a view of the Game Predictor form.
        /// </summary>
        /// <returns>The rendered view of the Game Predictor form.</returns>
        [HttpGet]
        public async Task<IActionResult> PredictGame()
        {
            var seasons = (await _seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Year);
            HttpContext.Session.SetObject("Seasons", seasons.ToList());
            ViewBag.GuestSeasons = new SelectList(seasons, "Year", "Year", GuestSeasonYear);
            ViewBag.HostSeasons = new SelectList(seasons, "Year", "Year", HostSeasonYear);

            var guestTeamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GuestSeasonYear);
            HttpContext.Session.SetObject("GuestTeamSeasons", guestTeamSeasons);
            ViewBag.Guests = new SelectList(guestTeamSeasons, "TeamName", "TeamName");

            var hostTeamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(HostSeasonYear);
            HttpContext.Session.SetObject("HostTeamSeasons", hostTeamSeasons);
            ViewBag.Hosts = new SelectList(hostTeamSeasons, "TeamName", "TeamName");

            return View();
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
        public async Task<IActionResult> PredictGame([Bind("GuestSeasonYear,GuestName,GuestScore,HostSeasonYear,HostName,HostScore")] GamePrediction prediction)
        {
            var seasons = HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GuestSeasonYear = prediction.GuestSeasonYear;
            ViewBag.GuestSeasons = new SelectList(orderedSeasons, "Year", "Year", GuestSeasonYear);

            var guestTeamSeasons = HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            var guest = guestTeamSeasons.FirstOrDefault(ts => ts.TeamName == prediction.GuestName);
            if (guest is null)
            {
                ViewBag.Guests = new SelectList(guestTeamSeasons, "TeamName", "TeamName");
            }
            else
            {
                ViewBag.Guests = new SelectList(guestTeamSeasons, "TeamName", "TeamName", guest.TeamName);
            }

            HostSeasonYear = prediction.HostSeasonYear;
            ViewBag.HostSeasons = new SelectList(orderedSeasons, "Year", "Year", HostSeasonYear);

            var hostTeamSeasons = HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            var host = hostTeamSeasons.FirstOrDefault(ts => ts.TeamName == prediction.HostName);
            if (host is null)
            {
                ViewBag.Hosts = new SelectList(hostTeamSeasons, "TeamName", "TeamName");
            }
            else
            {
                ViewBag.Hosts = new SelectList(hostTeamSeasons, "TeamName", "TeamName", host.TeamName);
            }

            GameScorePrediction gameScorePrediction = new GameScorePrediction();
            try
            {
                gameScorePrediction = _gamePredictorService.PredictGameScore(guest, host);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "A prediction could not be calculated for the selected teams.");
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
        public IActionResult ApplyFilter(int? guestSeasonYear, int? hostSeasonYear)
        {
            if (guestSeasonYear.HasValue)
            {
                GuestSeasonYear = guestSeasonYear.Value;
            }

            if (hostSeasonYear.HasValue)
            {
                HostSeasonYear = hostSeasonYear.Value;
            }

            return RedirectToAction(nameof(PredictGame));
        }
    }
}
