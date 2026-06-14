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
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueController"/> class.
    /// </remarks>
    /// <param name="leagueIndexViewModel">
    /// The <see cref="ILeagueIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="leagueDetailsViewModel">
    /// The <see cref="ILeaguesDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="leagueViewModelMapper">
    /// The <see cref="ILeagueViewModelMapper"/> by which league data will be mapped to view models.
    /// </param>
    /// <param name="leagueRepository">
    /// The <see cref="ILeagueRepository"/> by which league data will be accessed.
    /// </param>
    /// <param name="sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class LeagueController(
        ILeagueIndexViewModel leagueIndexViewModel,
        ILeagueDetailsViewModel leagueDetailsViewModel,
        ILeagueViewModelMapper leagueViewModelMapper,
        ILeagueRepository leagueRepository,
        ISharedRepository sharedRepository
        ) : Controller
    {
        // GET: Leagues
        /// <summary>
        /// Renders a view of the Leagues list.
        /// </summary>
        /// <returns>The rendered view of the Leagues list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leagues = await leagueRepository.GetLeaguesAsync();
            leagueIndexViewModel.Leagues = leagues
                .Select(l => leagueViewModelMapper.MapLeagueToViewModel(l))
                .OrderBy(l => l.ShortName)
                .ToList();

            return View(leagueIndexViewModel);
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

            var league = await leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            leagueDetailsViewModel.League = leagueViewModelMapper.MapLeagueToViewModel(league);

            return View(leagueDetailsViewModel);
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
        /// <param name="leagueViewModel">A <see cref="League"/> object with the data provided for the new league.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortName,LongName,FirstSeasonYear,LastSeasonYear")] LeagueViewModel leagueViewModel)
        {
            if (ModelState.IsValid)
            {
                var league = await leagueViewModelMapper.MapViewModelToLeague(leagueViewModel);
                await leagueRepository.AddAsync(league);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, league);
                    return View(leagueViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(leagueViewModel);
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

            var league = await leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            var leagueViewModel = new LeagueViewModel { League = league };
            return View(leagueViewModel);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the league edit form.
        /// </summary>
        /// <param name="leagueViewModel">A <see cref="League"/> object with the data provided for the league game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShortName,LongName,FirstSeasonYear,LastSeasonYear")] LeagueViewModel leagueViewModel)
        {
            if (id != leagueViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var league = await leagueViewModelMapper.MapViewModelToLeague(leagueViewModel);
                leagueRepository.Update(league);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await leagueRepository.LeagueExistsAsync(league.Id)))
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
                    return View(leagueViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(leagueViewModel);
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

            var league = await leagueRepository.GetLeagueAsync(id.Value);
            if (league is null)
            {
                return NotFound();
            }

            var leagueViewModel = leagueViewModelMapper.MapLeagueToViewModel(league);
            return View(leagueViewModel);
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
            await leagueRepository.DeleteAsync(id);
            await sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, League league)
        {
            var leagues = await leagueRepository.GetLeaguesAsync();
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
            return leagues.Any(c => c.Id == league.Id);
        }

        private bool UniqueKeyShortNameViolationExistsOnCreate(IEnumerable<League> leagues, League league)
        {
            return leagues.Any(c => c.ShortName == league.ShortName);
        }

        private bool UniqueKeyLongNameViolationExistsOnCreate(IEnumerable<League> leagues, League league)
        {
            return leagues.Any(c => c.LongName == league.LongName);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, League league)
        {
            var leagues = await leagueRepository.GetLeaguesAsync();
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
            return leagues.Count(c => c.ShortName == league.ShortName) > 1;
        }

        private bool UniqueKeyLongNameViolationExistsOnEdit(IEnumerable<League> leagues, League league)
        {
            return leagues.Count(c => c.LongName == league.LongName) > 1;
        }
    }
}
