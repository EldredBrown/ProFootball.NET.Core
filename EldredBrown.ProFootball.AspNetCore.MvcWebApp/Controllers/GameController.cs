using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS;
using Microsoft.AspNetCore.Http;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of game data.
    /// </summary>
    public class GameController : Controller
    {
        private readonly IGameIndexViewModel _gameIndexViewModel;
        private readonly IGameDetailsViewModel _gameDetailsViewModel;
        private readonly IGameViewModelMapper _gameViewModelMapper;
        private readonly IGameService _gameService;
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesController"/> class.
        /// </summary>
        /// <param name="gameIndexViewModel">
        /// The <see cref="IGameIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="gameDetailsViewModel">
        /// The <see cref="IGameDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="gameViewModelMapper">
        /// The <see cref="IGameViewModelMapper"/> by which game data will be mapped to view models.
        /// </param>
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
        public GameController(
            IGameIndexViewModel gamesIndexViewModel,
            IGameDetailsViewModel gamesDetailsViewModel,
            IGameViewModelMapper gameViewModelMapper,
            IGameService gameService,
            IGameRepository gameRepository,
            ITeamRepository teamRepository,
            ISeasonRepository seasonRepository,
            ISharedRepository sharedRepository
        )
        {
            _gameIndexViewModel = gamesIndexViewModel;
            _gameDetailsViewModel = gamesDetailsViewModel;
            _gameViewModelMapper = gameViewModelMapper;
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
            var seasons = await GetOrderedSeasons();
            var selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            if (selectedSeasonYear is null)
            {
                SetSelectedSeasonYear(seasons.First().Id);
                selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            }
            _gameIndexViewModel.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);
            _gameIndexViewModel.SelectedSeasonYear = selectedSeasonYear;

            var weeks = GetWeeks(seasons, selectedSeasonYear, firstIndex : 0);
            var selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");
            _gameIndexViewModel.Weeks = new SelectList(weeks, selectedWeek);
            _gameIndexViewModel.SelectedWeek = selectedWeek;

            var games = await GetGames(selectedSeasonYear, selectedWeek);
            _gameIndexViewModel.Games = games.Select(g => _gameViewModelMapper.MapGameToViewModel(g));

            return View(_gameIndexViewModel);
        }

        // GET: Games/Details/5
        /// <summary>
        /// Renders a view of the details of a selected game.
        /// </summary>
        /// <param name="id">The Id of the selected game.</param>
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

            _gameDetailsViewModel.Game = _gameViewModelMapper.MapGameToViewModel(game);

            return View(_gameDetailsViewModel);
        }

        // GET: Games/Create
        /// <summary>
        /// Renders a view of the game create form.
        /// </summary>
        /// <returns>The rendered view of the game create form.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var seasons = await GetOrderedSeasons();
            var selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            ViewBag.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);

            var weeks = GetWeeks(seasons, selectedSeasonYear, firstIndex: 0);
            var selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");
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
        /// <param name="gameViewModel">A <see cref="Game"/> object with the data provided for the new game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeasonYear,Week,GuestName,GuestScore,HostName,HostScore,IsPlayoff,Notes")] GameViewModel gameViewModel)
        {
            if (ModelState.IsValid)
            {
                var game = await _gameViewModelMapper.MapViewModelToGame(gameViewModel);
                await _gameService.AddGameAsync(game);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, game);
                    return View(gameViewModel);
                }

                SetSelectedWeek(game.Week);
                return RedirectToAction(nameof(Index));
            }

            var seasons = await GetOrderedSeasons();
            var selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            ViewBag.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);

            var weeks = GetWeeks(seasons, selectedSeasonYear, firstIndex: 0);
            var selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");
            ViewBag.Weeks = new SelectList(weeks, selectedWeek);

            return View(gameViewModel);
        }

        // GET: Games/Edit/5
        /// <summary>
        /// Renders a view of the game edit form.
        /// </summary>
        /// <returns>The rendered view of the game edit form.</returns>
        [HttpGet]
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

            var gameViewModel = _gameViewModelMapper.MapGameToViewModel(game);
            var selectedSeasonYear = gameViewModel.SeasonYear;

            var seasons = await GetOrderedSeasons();
            ViewBag.Seasons = new SelectList(seasons, "Id", "Id", selectedSeasonYear);

            int firstIndex = 1;
            var weeks = GetWeeks(seasons, selectedSeasonYear, firstIndex);
            ViewBag.Weeks = new SelectList(weeks, gameViewModel.Week);

            // TODO: Uncomment this when the slate of teams is finalized.
            //var teams = await _teamRepository.GetTeams();
            //ViewBag.GuestName = new SelectList(teams, "Name", "Name");
            //ViewBag.HostName = new SelectList(teams, "Name", "Name");

            //OldGame = game;
            HttpContext.Session.SetObject("OldGame", game);
            return View(gameViewModel);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the game edit form.
        /// </summary>
        /// <param name="gameViewModel">A <see cref="Game"/> object with the data provided for the game game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeasonYear,Week,GuestName,GuestScore,HostName,HostScore,IsPlayoff,Notes")] GameViewModel gameViewModel)
        {
            if (id != gameViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var game = await _gameViewModelMapper.MapViewModelToGame(gameViewModel);
                _gameRepository.Update(game);

                try
                {
                    var oldGame = HttpContext.Session.GetObject<Game>("OldGame");
                    await _gameService.EditGameAsync(game, oldGame!);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _gameRepository.GameExistsAsync(game.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnEdit(ex, game);
                    return View(gameViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(gameViewModel);
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

            var gameViewModel = _gameViewModelMapper.MapGameToViewModel(game);
            return View(gameViewModel);
        }

        // POST: Games/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a game.
        /// </summary>
        /// <param name="id">The Id of the game to delete.</param>
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

            HttpContext.Session.SetObject("SelectedSeasonYear", seasonYear);
            HttpContext.Session.SetObject<int?>("SelectedWeek", null);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected week.
        /// </summary>
        /// <param name="week">The selected week.</param>
        /// <returns>The rendered view of the <see cref="RedirectToActionResult"/>.</returns>
        public IActionResult SetSelectedWeek(int? week)
        {
            HttpContext.Session.SetObject("SelectedWeek", week);

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<Game>> GetGames(int? selectedSeasonYear, int? selectedWeek)
        {
            var games = (await _gameRepository.GetGamesAsync()).Where(g => g.SeasonId == selectedSeasonYear);
            if (selectedWeek.HasValue)
            {
                games = games.Where(g => g.Week == selectedWeek);
            }
            return games;
        }

        private async Task<IOrderedEnumerable<Season>> GetOrderedSeasons()
        {
            return (await _seasonRepository.GetSeasonsAsync())
                .OrderByDescending(s => s.Id);
        }

        private List<int?> GetWeeks(IEnumerable<Season> seasons, int? selectedSeasonYear, int firstIndex)
        {
            var weeks = new List<int?>();

            var selectedSeason = seasons.FirstOrDefault(s => s.Id == selectedSeasonYear);
            if (selectedSeason is not null)
            {
                for (int i = firstIndex; i <= selectedSeason.NumOfWeeksScheduled; i++)
                {
                    weeks.Add(i == 0 ? null : i);
                }
            }

            return weeks;
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Game game)
        {
            var games = await _gameRepository.GetGamesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(games, game))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A game with the same Id already exists.");
            }
            else if (UniqueKeyViolationExistsOnCreate(games, game))
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} A game with the same season, week, guest, and host already exists.");
            }
            else if (ForeignKeyUtils.ForeignKeyConstraintConflictExistsOnCreate(ex.InnerException.Message))
            {
                ForeignKeyUtils.AddModelErrorForForeignKeyConstraintConflict(errMsgIntro, ex.InnerException.Message,
                    ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} An unexpected error occurred.");
            }
        }

        private bool PrimaryKeyViolationExists(IEnumerable<Game> games, Game game)
        {
            return games.Any(g => g.Id == game.Id);
        }

        private bool UniqueKeyViolationExistsOnCreate(IEnumerable<Game> games, Game game)
        {
            return games.Any(
                g => g.SeasonId == game.SeasonId &&
                g.Week == game.Week &&
                g.GuestName == game.GuestName &&
                g.HostName == game.HostName
            );
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, Game game)
        {
            var games = await _gameRepository.GetGamesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyViolationExistsOnEdit(games, game))
            {
                ModelState.AddModelError(string.Empty,
                    $"{errMsgIntro} A game with the same season, week, guest, and host already exists.");
            }
            else if (ForeignKeyUtils.ForeignKeyConstraintConflictExistsOnEdit(ex.InnerException.Message))
            {
                ForeignKeyUtils.AddModelErrorForForeignKeyConstraintConflict(errMsgIntro, ex.InnerException.Message,
                    ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} An unexpected error occurred.");
            }
        }

        private bool UniqueKeyViolationExistsOnEdit(IEnumerable<Game> games, Game game)
        {
            return games.Count(
                g => g.SeasonId == game.SeasonId &&
                g.Week == game.Week &&
                g.GuestName == game.GuestName &&
                g.HostName == game.HostName
            ) > 1;
        }
    }
}
