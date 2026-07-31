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
    /// <param name="_leagueSeasonIndexViewModel">
    /// The <see cref="ILeagueSeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="_leagueSeasonDetailsViewModel">
    /// The <see cref="ILeagueSeasonsDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="_leagueSeasonViewModelMapper">
    /// The <see cref="ILeagueSeasonViewModelMapper"/> by which leagueSeason data will be mapped to view models.
    /// </param>
    /// <param name="_leagueSeasonRepository">
    /// The <see cref="ILeagueSeasonRepository"/> by which leagueSeason data will be accessed.
    /// </param>
    /// <param name="_sharedRepository">
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
        internal readonly ILeagueSeasonIndexViewModel _leagueSeasonIndexViewModel = leagueSeasonIndexViewModel;
        internal readonly ILeagueSeasonDetailsViewModel _leagueSeasonDetailsViewModel = leagueSeasonDetailsViewModel;
        internal readonly ILeagueSeasonViewModelMapper _leagueSeasonViewModelMapper = leagueSeasonViewModelMapper;
        internal readonly ILeagueSeasonRepository _leagueSeasonRepository = leagueSeasonRepository;
        internal readonly ISharedRepository _sharedRepository = sharedRepository;

        // GET: LeagueSeasons
        /// <summary>
        /// Renders a view of the LeagueSeasons list.
        /// </summary>
        /// <returns>The rendered view of the LeagueSeasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leagueSeasons = await _leagueSeasonRepository.GetLeagueSeasonsAsync();
            _leagueSeasonIndexViewModel.LeagueSeasons = 
                [.. leagueSeasons.Select(ls => _leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(ls))];

            return View(_leagueSeasonIndexViewModel);
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

            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
            if (leagueSeason is null)
            {
                return NotFound();
            }

            _leagueSeasonDetailsViewModel.LeagueSeason = _leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(
                leagueSeason);

            return View(_leagueSeasonDetailsViewModel);
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
        public async Task<IActionResult> Create(
            [Bind("LeagueName,SeasonYear,NumOfWeeksScheduled")] LeagueSeasonViewModel leagueSeasonViewModel
        )
        {
            if (ModelState.IsValid)
            {
                var leagueSeason = await _leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);
                await _leagueSeasonRepository.AddAsync(leagueSeason);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
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

            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
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
        public async Task<IActionResult> Edit(
            int id, [Bind("Id,LeagueName,SeasonYear,NumOfWeeksScheduled")] LeagueSeasonViewModel leagueSeasonViewModel
        )
        {
            if (id != leagueSeasonViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var leagueSeason = await _leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);
                _leagueSeasonRepository.Update(leagueSeason);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _leagueSeasonRepository.LeagueSeasonExistsAsync(leagueSeason.Id)))
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

            var leagueSeason = await _leagueSeasonRepository.GetLeagueSeasonAsync(id.Value);
            if (leagueSeason is null)
            {
                return NotFound();
            }

            var leagueSeasonViewModel = _leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason);
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
            await _leagueSeasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, LeagueSeason leagueSeason)
        {
            var leagueSeasons = await _leagueSeasonRepository.GetLeagueSeasonsAsync();

            if (PrimaryKeyViolationExists(leagueSeasons, leagueSeason))
            {
                ModelState.AddModelError("Id", $"{DbVerificationUtils.ErrMsgIntro} A LeagueSeason with the same Id already exists.");
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

            if (DbVerificationUtils.UniqueKeyConstraintExists(ex.InnerException.Message))
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

        private static bool PrimaryKeyViolationExists(IEnumerable<LeagueSeason> leagueSeasons, LeagueSeason leagueSeason)
        {
            return leagueSeasons.Any(ls => ls.Id == leagueSeason.Id);
        }
    }
}
