using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of leagueSeason data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueSeasonController"/> class.
    /// </remarks>
    /// <param name="leagueSeasonIndexViewModel">
    /// The <see cref="ILeagueSeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="leagueSeasonDetailsViewModel">
    /// The <see cref="ILeagueSeasonsDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="leagueSeasonViewModelMapper">
    /// The <see cref="ILeagueSeasonViewModelMapper"/> by which leagueSeason data will be mapped to view models.
    /// </param>
    /// <param name="leagueSeasonRepository">
    /// The <see cref="ILeagueSeasonRepository"/> by which leagueSeason data will be accessed.
    /// </param>
    /// <param name="sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class LeagueSeasonController(
        ILeagueSeasonIndexViewModel leagueSeasonIndexViewModel,
        ILeagueSeasonDetailsViewModel leagueSeasonDetailsViewModel,
        ILeagueSeasonViewModelMapper leagueSeasonViewModelMapper,
        ILeagueSeasonRepository leagueSeasonRepository,
        ISharedRepository sharedRepository
        ) : Controller
    {

        // GET: LeagueSeasons
        /// <summary>
        /// Renders a view of the LeagueSeasons list.
        /// </summary>
        /// <returns>The rendered view of the LeagueSeasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leagueSeasons = await leagueSeasonRepository.GetLeagueSeasonsAsync();
            leagueSeasonIndexViewModel.LeagueSeasons = leagueSeasons
                .Select(ls => leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(ls))
                .ToList();

            return View(leagueSeasonIndexViewModel);
        }

        // GET: LeagueSeasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected leagueSeason.
        /// </summary>
        /// <param name="id">The Id of the selected leagueSeason.</param>
        /// <returns>The rendered view of the selected leagueSeason.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
            if (leagueSeason is null)
            {
                return NotFound();
            }

            leagueSeasonDetailsViewModel.LeagueSeason = leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(
                leagueSeason);

            return View(leagueSeasonDetailsViewModel);
        }

        // GET: LeagueSeasons/Create
        /// <summary>
        /// Renders a view of the leagueSeason create form.
        /// </summary>
        /// <returns>The rendered view of the leagueSeason create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeagueSeasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the leagueSeason create form.
        /// </summary>
        /// <param name="leagueSeasonViewModel">A <see cref="LeagueSeason"/> object with the data provided for the new leagueSeason.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeagueName,SeasonYear")] LeagueSeasonViewModel leagueSeasonViewModel)
        {
            if (ModelState.IsValid)
            {
                var leagueSeason = await leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);
                await leagueSeasonRepository.AddAsync(leagueSeason);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, leagueSeason);
                    return View(leagueSeasonViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(leagueSeasonViewModel);
        }

        // GET: LeagueSeasons/Edit/5
        /// <summary>
        /// Renders a view of the leagueSeason edit form.
        /// </summary>
        /// <returns>The rendered view of the leagueSeason edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
            if (leagueSeason is null)
            {
                return NotFound();
            }

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            return View(leagueSeasonViewModel);
        }

        // POST: LeagueSeasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the leagueSeason edit form.
        /// </summary>
        /// <param name="leagueSeasonViewModel">A <see cref="LeagueSeason"/> object with the data provided for the leagueSeason game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LeagueName,SeasonYear")] LeagueSeasonViewModel leagueSeasonViewModel)
        {
            if (id != leagueSeasonViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var leagueSeason = await leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);
                leagueSeasonRepository.Update(leagueSeason);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await leagueSeasonRepository.LeagueSeasonExistsAsync(leagueSeason.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex, leagueSeason);
                    return View(leagueSeasonViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(leagueSeasonViewModel);
        }

        // GET: LeagueSeasons/Delete/5
        /// <summary>
        /// Renders a view of the leagueSeason delete form.
        /// </summary>
        /// <returns>The rendered view of the leagueSeason delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
            if (leagueSeason is null)
            {
                return NotFound();
            }

            var leagueSeasonViewModel = leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason);
            return View(leagueSeasonViewModel);
        }

        // POST: LeagueSeasons/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a leagueSeason.
        /// </summary>
        /// <param name="id">The Id of the leagueSeason to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await leagueSeasonRepository.DeleteAsync(id);
            await sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, LeagueSeason leagueSeason)
        {
            var leagueSeasons = await leagueSeasonRepository.GetLeagueSeasonsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(leagueSeasons, leagueSeason))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A LeagueSeason with the same Id already exists.");
            }
            else if (UniqueKeyViolationExistsOnCreate(leagueSeasons, leagueSeason))
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} A LeagueSeason with the same league name and season year already exists.");
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

        private bool PrimaryKeyViolationExists(IEnumerable<LeagueSeason> leagueSeasons, LeagueSeason leagueSeason)
        {
            return leagueSeasons.Any(ts => ts.Id == leagueSeason.Id);
        }

        private bool UniqueKeyViolationExistsOnCreate(IEnumerable<LeagueSeason> leagueSeasons, LeagueSeason leagueSeason)
        {
            return leagueSeasons.Any(ts => ts.LeagueId == leagueSeason.LeagueId && ts.SeasonId == leagueSeason.SeasonId);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, LeagueSeason leagueSeason)
        {
            var leagueSeasons = await leagueSeasonRepository.GetLeagueSeasonsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyViolationExistsOnEdit(leagueSeasons, leagueSeason))
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} A LeagueSeason with the same league name and season year already exists.");
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

        private bool UniqueKeyViolationExistsOnEdit(IEnumerable<LeagueSeason> leagueSeasons, LeagueSeason leagueSeason)
        {
            return leagueSeasons.Count(ts => ts.LeagueId == leagueSeason.LeagueId && ts.SeasonId == leagueSeason.SeasonId) > 1;
        }
    }
}
