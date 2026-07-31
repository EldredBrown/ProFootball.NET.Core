using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;
using Microsoft.IdentityModel.Tokens;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of game data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GamesController"/> class.
    /// </remarks>
    /// <param name="_gameIndexViewModel">
    /// The <see cref="IGameIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="_gameDetailsViewModel">
    /// The <see cref="IGameDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="_gameViewModelMapper">
    /// The <see cref="ITeamSeasonViewModelMapper"/> by which game data will be mapped to view models.
    /// </param>
    /// <param name="_gameService">
    /// The <see cref="IGameService"/> for processing Game data.
    /// </param>
    /// <param name="_gameRepository">
    /// The <see cref="IGameRepository"/> by which game data will be accessed.
    /// </param>
    /// <param name="_teamRepository">
    /// The <see cref="ITeamRepository"/> by which team data will be accessed.
    /// </param>
    /// <param name="_seasonRepository">
    /// The <see cref="IAssociationRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="_sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    public class GameController(
        IGameIndexViewModel gameIndexViewModel,
        IGameDetailsViewModel gameDetailsViewModel,
        IGameViewModelMapper gameViewModelMapper,
        IGameService gameService,
        ISeasonRepository seasonRepository,
        IAssociationRepository associationRepository,
        ITeamRepository teamRepository,
        IGameRepository gameRepository,
        ILeagueSeasonRepository leagueSeasonRepository,
        ISharedRepository sharedRepository
    ) : Controller
    {
        internal readonly IGameIndexViewModel _gameIndexViewModel = gameIndexViewModel;
        internal readonly IGameDetailsViewModel _gameDetailsViewModel = gameDetailsViewModel;
        internal readonly IGameViewModelMapper _gameViewModelMapper = gameViewModelMapper;
        internal readonly IGameService _gameService = gameService;
        internal readonly ISeasonRepository _seasonRepository = seasonRepository;
        internal readonly IAssociationRepository _associationRepository = associationRepository;
        internal readonly ITeamRepository _teamRepository = teamRepository;
        internal readonly IGameRepository _gameRepository = gameRepository;
        internal readonly ILeagueSeasonRepository _leagueSeasonRepository = leagueSeasonRepository;
        internal readonly ISharedRepository _sharedRepository = sharedRepository;

        private int? _selectedSeasonYear = null;
        private string _selectedLeagueName = string.Empty;
        private int? _selectedWeek = null;

        // GET: Games
        /// <summary>
        /// Renders a view of the Games list.
        /// </summary>
        /// <returns>The rendered view of the Games list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel();
            await LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel();
            await LoadWeeksAndSelectedWeekIntoIndexViewModel();
            await SetIndexViewModelGames();
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
            _selectedSeasonYear = HttpContext.Session.GetObject<int>("SelectedSeasonYear");
            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            _selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");

            await LoadSeasonsIntoViewBag();
            await LoadLeaguesIntoViewBag();
            await LoadWeeksIntoViewBag();

            // TODO: Uncomment this when the slate of teams is finalized.
            //var teams = await __teamRepository.GetTeams();
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
        public async Task<IActionResult> Create(
            [Bind("SeasonYear,LeagueName,Week,GuestName,GuestScore,HostName,HostScore,IsPlayoff,Notes")] GameViewModel gameViewModel
        )
        {
            _selectedSeasonYear = HttpContext.Session.GetObject<int>("SelectedSeasonYear");
            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            _selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");

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
                    await SetUpView(gameViewModel);
                    return View(gameViewModel);
                }

                SetSelectedWeek(game.Week);
                return RedirectToAction(nameof(Index));
            }

            await SetUpView(gameViewModel);
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

            _selectedSeasonYear = gameViewModel.SeasonYear;
            _selectedLeagueName = gameViewModel.LeagueName;
            _selectedWeek = gameViewModel.Week;

            await LoadSeasonsIntoViewBag();
            await LoadLeaguesIntoViewBag();
            await LoadWeeksIntoViewBag();

            // TODO: Uncomment this when the slate of teams is finalized.
            //var teams = await __teamRepository.GetTeams();
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
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,SeasonYear,LeagueName,Week,GuestName,GuestScore,HostName,HostScore,IsPlayoff,Notes")] GameViewModel gameViewModel
        )
        {
            _selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            _selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");

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
                    await HandleDbUpdateExceptionOnEdit(ex);
                    await SetUpView(gameViewModel);
                    return View(gameViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            await SetUpView(gameViewModel);
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

        private async Task SetIndexViewModelGames()
        {
            var games = await GetGames();
            _gameIndexViewModel.Games = [.. games.Select(g => _gameViewModelMapper.MapGameToViewModel(g))];
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

            _selectedSeasonYear = seasonYear.Value;
            HttpContext.Session.SetObject("SelectedSeasonYear", seasonYear.Value);

            _selectedLeagueName = string.Empty;
            HttpContext.Session.SetObject("SelectedLeagueName", string.Empty);

            _selectedWeek = null;
            HttpContext.Session.SetObject<int?>("SelectedWeek", null);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the selected league name.
        /// </summary>
        /// <param name="leagueName">The name to which the selected league name will be set.</param>
        /// <returns>The rendered view of the team seasons index.</returns>
        public IActionResult SetSelectedLeagueName(string leagueName)
        {
            if (leagueName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            _selectedLeagueName = leagueName;
            HttpContext.Session.SetObject("SelectedLeagueName", leagueName);

            _selectedWeek = null;
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
            _selectedWeek = week;
            HttpContext.Session.SetObject("SelectedWeek", week);

            return RedirectToAction(nameof(Index));
        }

        private void AddModelErrorForStringTooLong(DbUpdateException ex)
        {
            string columnName = DbVerificationUtils.GetColumnNameFromDbUpdateException(ex);
            switch (columnName)
            {
                case "'guest_name'":
                    DbVerificationUtils.AddModelErrorForStringTooLong(ModelState, "GuestName");
                    break;
                case "'host_name'":
                    DbVerificationUtils.AddModelErrorForStringTooLong(ModelState, "HostName");
                    break;
                default:
                    break;
            }
        }

        private async Task<IEnumerable<Game>> GetGames()
        {
            var selectedLeague = await _associationRepository.GetAssociationByShortNameAsync(_selectedLeagueName);
            var games = (await _gameRepository.GetGamesBySeasonLeagueAndWeekAsync(_selectedSeasonYear.Value, 
                selectedLeague?.Id, _selectedWeek)).ToList();
            return games;
        }

        private async Task<IEnumerable<Association>> GetLeagues()
        {
            return [.. (await _associationRepository.GetAssociationsAsync())
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= _selectedSeasonYear
                    && (l.LastSeasonYearNavigation is null || _selectedSeasonYear <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)];
        }

        private async Task<IEnumerable<Season>> GetSeasons()
        {
            return [.. (await _seasonRepository.GetSeasonsAsync()).OrderByDescending(s => s.Year)];
        }

        private async Task<List<int?>> GetWeeks(int firstIndex)
        {
            //_selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            var selectedLeague = await _associationRepository.GetAssociationByShortNameAsync(_selectedLeagueName);
            var selectedLeagueId = (selectedLeague?.Id) ?? 
                throw new KeyNotFoundException("A league with the specified short name was not found.");
            var selectedLeagueSeason = 
                await _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeagueId, _selectedSeasonYear.Value);

            var weeks = new List<int?>();
            if (selectedLeagueSeason is not null)
            {
                for (int i = firstIndex; i <= selectedLeagueSeason.NumOfWeeksScheduled; i++)
                {
                    weeks.Add(i == 0 ? null : i);
                }
            }

            return weeks;
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Game game)
        {
            var games = await _gameRepository.GetGamesAsync();

            if (PrimaryKeyViolationExists(games, game))
            {
                ModelState.AddModelError("Id", $"{DbVerificationUtils.ErrMsgIntro} A game with the same Id already exists.");
            }
            else
            {
                await HandleDbUpdateExceptionOnEdit(ex, DbVerificationUtils.SqlOperation.INSERT);
            }
        }

        private async Task HandleDbUpdateExceptionOnEdit(
            DbUpdateException ex, DbVerificationUtils.SqlOperation? sqlOperation = null
        )
        {
            sqlOperation ??= DbVerificationUtils.SqlOperation.UPDATE;

            if (DbVerificationUtils.StringTooLong(ex))
            {
                AddModelErrorForStringTooLong(ex);
            }
            else if (DbVerificationUtils.UniqueKeyConstraintExists(ex.InnerException.Message))
            {
                DbVerificationUtils.AddModelErrorForUniqueKeyConstraintConflict(ModelState);
            }
            else if (
                DbVerificationUtils.ForeignKeyConstraintConflictExists(sqlOperation.ToString(), ex.InnerException.Message)
            )
            {
                DbVerificationUtils.AddModelErrorForForeignKeyConstraintConflict(ModelState, ex.InnerException.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{DbVerificationUtils.ErrMsgIntro} An unexpected error occurred.");
            }
        }

        private async Task LoadLeaguesAndSelectedLeagueNameIntoIndexViewModel()
        {
            var leagues = await GetLeagues();

            _selectedLeagueName = HttpContext.Session.GetObject<string>("SelectedLeagueName");
            if (_selectedLeagueName.IsNullOrEmpty())
            {
                SetSelectedLeagueName(leagues.First().ShortName);
            }
            _gameIndexViewModel.Leagues = new SelectList(leagues, "ShortName", "ShortName", _selectedLeagueName);
            _gameIndexViewModel.SelectedLeagueName = _selectedLeagueName;
        }

        private async Task LoadLeaguesIntoViewBag()
        {
            var leagues = await GetLeagues();
            ViewBag.Leagues = new SelectList(leagues, "ShortName", "ShortName", _selectedLeagueName);
        }

        private async Task LoadSeasonsAndSelectedSeasonYearIntoIndexViewModel()
        {
            var seasons = await GetSeasons();

            _selectedSeasonYear = HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            if (_selectedSeasonYear is null)
            {
                SetSelectedSeasonYear(seasons.First().Year);
            }
            _gameIndexViewModel.Seasons = new SelectList(seasons, "Year", "Year", _selectedSeasonYear);
            _gameIndexViewModel.SelectedSeasonYear = _selectedSeasonYear;
        }

        private async Task LoadSeasonsIntoViewBag()
        {
            var seasons = await GetSeasons();
            ViewBag.Seasons = new SelectList(seasons, "Year", "Year", _selectedSeasonYear);
        }

        private async Task LoadWeeksAndSelectedWeekIntoIndexViewModel()
        {
            var weeks = await GetWeeks(firstIndex: 0);

            _selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");
            _gameIndexViewModel.Weeks = new SelectList(weeks, _selectedWeek);
            _gameIndexViewModel.SelectedWeek = _selectedWeek;
        }

        private async Task LoadWeeksIntoViewBag()
        {
            var weeks = await GetWeeks(firstIndex: 1);
            ViewBag.Weeks = new SelectList(weeks, _selectedWeek);
        }

        private static bool PrimaryKeyViolationExists(IEnumerable<Game> games, Game game)
        {
            return games.Any(g => g.Id == game.Id);
        }

        private async Task SetUpView(GameViewModel gameViewModel)
        {
            var seasons = await GetSeasons();
            ViewBag.Seasons = new SelectList(seasons, "Year", "Year", gameViewModel.SeasonYear);

            var leagues = await GetLeagues();
            ViewBag.Leagues = new SelectList(leagues, "ShortName", "ShortName", gameViewModel.LeagueName);

            var weeks = await GetWeeks(firstIndex: 1);
            var selectedWeek = HttpContext.Session.GetObject<int?>("SelectedWeek");
            ViewBag.Weeks = new SelectList(weeks, selectedWeek);
        }
    }
}
