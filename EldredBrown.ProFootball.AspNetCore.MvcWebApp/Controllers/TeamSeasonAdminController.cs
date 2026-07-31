using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of teamSeason data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamSeasonController"/> class.
    /// </remarks>
    /// <param name="teamSeasonIndexViewModel">
    /// The <see cref="ITeamSeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="teamSeasonDetailsViewModel">
    /// The <see cref="ITeamSeasonsDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="teamSeasonViewModelMapper">
    /// The <see cref="ITeamSeasonViewModelMapper"/> by which teamSeason data will be mapped to view models.
    /// </param>
    /// <param name="teamSeasonRepository">
    /// The <see cref="ITeamSeasonRepository"/> by which teamSeason data will be accessed.
    /// </param>
    /// <param name="sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class TeamSeasonAdminController(
        ITeamSeasonIndexViewModel teamSeasonIndexViewModel,
        ITeamSeasonDetailsViewModel teamSeasonDetailsViewModel,
        ITeamSeasonViewModelMapper teamSeasonViewModelMapper,
        ITeamSeasonRepository teamSeasonRepository,
        ISharedRepository sharedRepository
    ) : Controller
    {
        internal readonly ITeamSeasonIndexViewModel _teamSeasonIndexViewModel = teamSeasonIndexViewModel;
        internal readonly ITeamSeasonDetailsViewModel _teamSeasonDetailsViewModel = teamSeasonDetailsViewModel;
        internal readonly ITeamSeasonViewModelMapper _teamSeasonViewModelMapper = teamSeasonViewModelMapper;
        internal readonly ITeamSeasonRepository _teamSeasonRepository = teamSeasonRepository;
        internal readonly ISharedRepository _sharedRepository = sharedRepository;

        // GET: TeamSeasons
        /// <summary>
        /// Renders a view of the TeamSeasons list.
        /// </summary>
        /// <returns>The rendered view of the TeamSeasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsAsync();
            _teamSeasonIndexViewModel.TeamSeasons = 
                [.. teamSeasons.Select(ts => _teamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))];

            return View(_teamSeasonIndexViewModel);
        }

        // GET: TeamSeasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected teamSeason.
        /// </summary>
        /// <param name="id">The Id of the selected teamSeason.</param>
        /// <returns>The rendered view of the selected teamSeason.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var teamSeason = await _teamSeasonRepository.GetTeamSeasonAsync(id.Value);
            if (teamSeason is null)
            {
                return NotFound();
            }

            _teamSeasonDetailsViewModel.TeamSeason = _teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason);

            return View(_teamSeasonDetailsViewModel);
        }

        // GET: TeamSeasons/Create
        /// <summary>
        /// Renders a view of the teamSeason create form.
        /// </summary>
        /// <returns>The rendered view of the teamSeason create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TeamSeasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the teamSeason create form.
        /// </summary>
        /// <param name="teamSeasonViewModel">A <see cref="TeamSeason"/> object with the data provided for the new teamSeason.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TeamName,SeasonYear,AssociationName")] TeamSeasonViewModel teamSeasonViewModel
        )
        {
            if (ModelState.IsValid)
            {
                var teamSeason = await _teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel);
                await _teamSeasonRepository.AddAsync(teamSeason);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, teamSeason);
                    return View(teamSeasonViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(teamSeasonViewModel);
        }

        // GET: TeamSeasons/Edit/5
        /// <summary>
        /// Renders a view of the teamSeason edit form.
        /// </summary>
        /// <returns>The rendered view of the teamSeason edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var teamSeason = await _teamSeasonRepository.GetTeamSeasonAsync(id.Value);
            if (teamSeason is null)
            {
                return NotFound();
            }

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            return View(teamSeasonViewModel);
        }

        // POST: TeamSeasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the teamSeason edit form.
        /// </summary>
        /// <param name="teamSeasonViewModel">A <see cref="TeamSeason"/> object with the data provided for the teamSeason game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, [Bind("Id,TeamName,SeasonYear,AssociationName")] TeamSeasonViewModel teamSeasonViewModel
        )
        {
            if (id != teamSeasonViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var teamSeason = await _teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel);
                _teamSeasonRepository.Update(teamSeason);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _teamSeasonRepository.TeamSeasonExistsAsync(teamSeason.Id)))
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
                    return View(teamSeasonViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(teamSeasonViewModel);
        }

        // GET: TeamSeasons/Delete/5
        /// <summary>
        /// Renders a view of the teamSeason delete form.
        /// </summary>
        /// <returns>The rendered view of the teamSeason delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var teamSeason = await _teamSeasonRepository.GetTeamSeasonAsync(id.Value);
            if (teamSeason is null)
            {
                return NotFound();
            }

            var teamSeasonViewModel = _teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason);
            return View(teamSeasonViewModel);
        }

        // POST: TeamSeasons/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a teamSeason.
        /// </summary>
        /// <param name="id">The Id of the teamSeason to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamSeasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, TeamSeason teamSeason)
        {
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsAsync();

            if (PrimaryKeyViolationExists(teamSeasons, teamSeason))
            {
                ModelState.AddModelError("Id", $"{DbVerificationUtils.ErrMsgIntro} A TeamSeason with the same Id already exists.");
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

        private static bool PrimaryKeyViolationExists(IEnumerable<TeamSeason> teamSeasons, TeamSeason teamSeason)
        {
            return teamSeasons.Any(ts => ts.Id == teamSeason.Id);
        }
    }
}
