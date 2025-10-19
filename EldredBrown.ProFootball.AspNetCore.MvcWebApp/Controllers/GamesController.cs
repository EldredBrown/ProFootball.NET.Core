using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Games;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of game data.
    /// </summary>
    public class GamesController : Controller
    {
        public static int SelectedSeasonYear = 1920;
        public static int? SelectedWeek;
        public static Game OldGame;

        private readonly IGamesIndexViewModel _gamesIndexViewModel;
        private readonly IGamesDetailsViewModel _gamesDetailsViewModel;
        private readonly IGameService _gameService;
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesController"/> class.
        /// </summary>
        /// <param name="gameService">
        /// The <see cref="IGameService"/> for processing Game data.
        /// </param>
        /// <param name="gameRepository">
        /// The <see cref="IGameRepository"/> by which game data will be accessed.
        /// </param>
        /// <param name="teamRepository">
        /// The <see cref="ITeamRepository"/> by which team data will be accessed.
        /// </param>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public GamesController(
            IGamesIndexViewModel gamesIndexViewModel,
            IGamesDetailsViewModel gamesDetailsViewModel,
            IGameService gameService,
            IGameRepository gameRepository,
            ITeamRepository teamRepository,
            ISeasonRepository seasonRepository,
            ISharedRepository sharedRepository)
        {
            _gamesIndexViewModel = gamesIndexViewModel;
            _gamesDetailsViewModel = gamesDetailsViewModel;
            _gameService = gameService;
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _seasonRepository = seasonRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Games
        /// <summary>
        /// Renders a view of the Games list.
        /// </summary>
        /// <returns>The rendered view of the Games list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (seasons, orderedSeasons) = await GetSeasonsAndOrderedSeasons();
            _gamesIndexViewModel.Seasons = new SelectList(orderedSeasons, "Year", "Year", SelectedSeasonYear);
            _gamesIndexViewModel.SelectedSeasonYear = SelectedSeasonYear;

            var weeks = GetWeeks(seasons, firstIndex: 0);
            _gamesIndexViewModel.Weeks = new SelectList(weeks, SelectedWeek);
            _gamesIndexViewModel.SelectedWeek = SelectedWeek;

            var games = await GetGames();
            _gamesIndexViewModel.Games = games;

            return View(_gamesIndexViewModel);
        }

        // GET: Games/Details/5
        /// <summary>
        /// Renders a view of the details of a selected game.
        /// </summary>
        /// <param name="id">The ID of the selected game.</param>
        /// <returns>The rendered view of the selected game.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var game = await _gameRepository.GetGameAsync(id.Value);
            if (game is null)
            {
                return NotFound();
            }

            _gamesDetailsViewModel.Game = game;

            return View(_gamesDetailsViewModel);
        }

        // GET: Games/Create
        /// <summary>
        /// Renders a view of the game create form.
        /// </summary>
        /// <returns>The rendered view of the game create form.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var (seasons, orderedSeasons) = await GetSeasonsAndOrderedSeasons();
            ViewBag.Seasons = new SelectList(orderedSeasons, "Year", "Year", SelectedSeasonYear);

            int firstIndex = 1;
            var weeks = GetWeeks(seasons, firstIndex);
            int selectedWeek = SelectedWeek ?? firstIndex;
            ViewBag.Weeks = new SelectList(weeks, selectedWeek);

            // TODO: Uncomment this when the slate of teams is finalized.
            //var teams = await _teamRepository.GetTeams();
            //ViewBag.GuestName = new SelectList(teams, "Name", "Name");
            //ViewBag.HostName = new SelectList(teams, "Name", "Name");

            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the game create form.
        /// </summary>
        /// <param name="game">A <see cref="Game"/> object with the data provided for the new game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeasonYear,Week,GuestName,GuestScore,HostName,HostScore,WinnerName,WinnerScore,LoserName,LoserScore,IsPlayoffGame,Notes")] Game game)
        {
            if (ModelState.IsValid)
            {
                await _gameService.AddGameAsync(game);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Create));
            }

            return View(game);
        }

        // GET: Games/Edit/5
        [HttpGet]
        /// <summary>
        /// Renders a view of the game edit form.
        /// </summary>
        /// <returns>The rendered view of the game edit form.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var game = await _gameRepository.GetGameAsync(id.Value);
            if (game is null)
            {
                return NotFound();
            }

            var (seasons, orderedSeasons) = await GetSeasonsAndOrderedSeasons();
            ViewBag.Seasons = new SelectList(orderedSeasons, "Year", "Year", game.SeasonYear);

            int firstIndex = 1;
            var weeks = GetWeeks(seasons, firstIndex);
            ViewBag.Weeks = new SelectList(weeks, game.Week);

            // TODO: Uncomment this when the slate of teams is finalized.
            //var teams = await _teamRepository.GetTeams();
            //ViewBag.GuestName = new SelectList(teams, "Name", "Name");
            //ViewBag.HostName = new SelectList(teams, "Name", "Name");

            OldGame = game;

            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the game edit form.
        /// </summary>
        /// <param name="game">A <see cref="Game"/> object with the data provided for the updated game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SeasonYear,Week,GuestName,GuestScore,HostName,HostScore,WinnerName,WinnerScore,LoserName,LoserScore,IsPlayoffGame,Notes")] Game game)
        {
            if (id != game.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _gameService.EditGameAsync(game, OldGame!);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _gameRepository.GameExists(game.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(game);
        }

        // GET: Games/Delete/5
        /// <summary>
        /// Renders a view of the game delete form.
        /// </summary>
        /// <returns>The rendered view of the game delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var game = await _gameRepository.GetGameAsync(id.Value);
            if (game is null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a game.
        /// </summary>
        /// <param name="id">The ID of the game to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _gameService.DeleteGameAsync(id);
            await _sharedRepository.SaveChangesAsync();

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

        /// <summary>
        /// Sets the selected week.
        /// </summary>
        /// <param name="week">The selected week.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedWeek(int? week)
        {
            SelectedWeek = week;

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<Game>> GetGames()
        {
            var games = (await _gameRepository.GetGamesAsync()).Where(g => g.SeasonYear == SelectedSeasonYear);
            if (SelectedWeek.HasValue)
            {
                games = games.Where(g => g.Week == SelectedWeek);
            }
            games = games.ToList();

            return games;
        }

        private async Task<(IEnumerable<Season>, IOrderedEnumerable<Season>)> GetSeasonsAndOrderedSeasons()
        {
            var seasons = await _seasonRepository.GetSeasonsAsync();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year);
            return (seasons, orderedSeasons);
        }

        private static List<int?> GetWeeks(IEnumerable<Season> seasons, int firstIndex)
        {
            var weeks = new List<int?>();

            var selectedSeason = seasons.FirstOrDefault(s => s.Year == SelectedSeasonYear);
            if (!(selectedSeason is null))
            {
                for (int i = firstIndex; i <= selectedSeason.NumOfWeeksScheduled; i++)
                {
                    if (i == 0)
                    {
                        weeks.Add(null);
                    }
                    else
                    {
                        weeks.Add(i);
                    }
                }
            }

            return weeks;
        }
    }
}
