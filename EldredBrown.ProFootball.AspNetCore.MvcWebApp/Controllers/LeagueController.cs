using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of league data.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    public class LeagueController : Controller
    {
        private readonly ILeagueIndexViewModel _leagueIndexViewModel;
        private readonly ILeagueDetailsViewModel _leagueDetailsViewModel;
        private readonly ILeagueRepository _leagueRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueController"/> class.
        /// </summary>
        /// <param name="leagueIndexViewModel">
        /// The <see cref="ILeagueIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="leagueDetailsViewModel">
        /// The <see cref="ILeaguesDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="leagueRepository">
        /// The <see cref="ILeagueRepository"/> by which league data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public LeagueController(
            ILeagueIndexViewModel leagueIndexViewModel,
            ILeagueDetailsViewModel leagueDetailsViewModel,
            ILeagueRepository leagueRepository,
            ISharedRepository sharedRepository
        )
        {
            _leagueIndexViewModel = leagueIndexViewModel;
            _leagueDetailsViewModel = leagueDetailsViewModel;
            _leagueRepository = leagueRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Leagues
        /// <summary>
        /// Renders a view of the Leagues list.
        /// </summary>
        /// <returns>The rendered view of the Leagues list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _leagueIndexViewModel.Leagues = await _leagueRepository.GetLeaguesAsync();

            return View(_leagueIndexViewModel);
        }

        // GET: Leagues/Details/5
        /// <summary>
        /// Renders a view of the details of a selected league.
        /// </summary>
        /// <param name="id">The Id of the selected league.</param>
        /// <returns>The rendered view of the selected league.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var league = await _leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            _leagueDetailsViewModel.League = league;

            return View(_leagueDetailsViewModel);
        }

        // GET: Leagues/Create
        /// <summary>
        /// Renders a view of the league create form.
        /// </summary>
        /// <returns>The rendered view of the league create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the league create form.
        /// </summary>
        /// <param name="league">A <see cref="League"/> object with the data provided for the new league.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortName,LongName,FirstSeasonId,LastSeasonId")] League league)
        {
            if (ModelState.IsValid)
            {
                await _leagueRepository.AddAsync(league);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, league);
                    return View(league);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(league);
        }

        // GET: Leagues/Edit/5
        /// <summary>
        /// Renders a view of the league edit form.
        /// </summary>
        /// <returns>The rendered view of the league edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var league = await _leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the league edit form.
        /// </summary>
        /// <param name="league">A <see cref="League"/> object with the data provided for the league game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShortName,LongName,FirstSeasonId,LastSeasonId")] League league)
        {
            if (id != league.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _leagueRepository.Update(league);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _leagueRepository.LeagueExistsAsync(league.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex, league);
                    return View(league);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(league);
        }

        // GET: Leagues/Delete/5
        /// <summary>
        /// Renders a view of the league delete form.
        /// </summary>
        /// <returns>The rendered view of the league delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var league = await _leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            return View(league);
        }

        // POST: Leagues/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a league.
        /// </summary>
        /// <param name="id">The Id of the league to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _leagueRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, League league)
        {
            var leagues = await _leagueRepository.GetLeaguesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(leagues, league))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A league with the same Id already exists.");
            }
            else if (UniqueKeyShortNameViolationExistsOnCreate(leagues, league))
            {
                ModelState.AddModelError("ShortName", $"{errMsgIntro} A league with the same short name already exists.");
            }
            else if (UniqueKeyLongNameViolationExistsOnCreate(leagues, league))
            {
                ModelState.AddModelError("LongName", $"{errMsgIntro} A league with the same long name already exists.");
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

        private bool PrimaryKeyViolationExists(IEnumerable<League> leagues, League league)
        {
            return leagues.Any(l => l.Id == league.Id);
        }

        private bool UniqueKeyShortNameViolationExistsOnCreate(IEnumerable<League> leagues, League league)
        {
            return leagues.Any(l => l.ShortName == league.ShortName);
        }

        private bool UniqueKeyLongNameViolationExistsOnCreate(IEnumerable<League> leagues, League league)
        {
            return leagues.Any(l => l.LongName == league.LongName);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, League league)
        {
            var leagues = await _leagueRepository.GetLeaguesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyShortNameViolationExistsOnEdit(leagues, league))
            {
                ModelState.AddModelError("ShortName", $"{errMsgIntro} A league with the same short name already exists.");
            }
            else if (UniqueKeyLongNameViolationExistsOnEdit(leagues, league))
            {
                ModelState.AddModelError("LongName", $"{errMsgIntro} A league with the same long name already exists.");
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

        private bool UniqueKeyShortNameViolationExistsOnEdit(IEnumerable<League> leagues, League league)
        {
            return leagues.Count(l => l.ShortName == league.ShortName) > 1;
        }

        private bool UniqueKeyLongNameViolationExistsOnEdit(IEnumerable<League> leagues, League league)
        {
            return leagues.Count(l => l.LongName == league.LongName) > 1;
        }
    }
}
