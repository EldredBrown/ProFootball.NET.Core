using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of division data.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    public class DivisionController : Controller
    {
        private readonly IDivisionIndexViewModel _divisionIndexViewModel;
        private readonly IDivisionDetailsViewModel _divisionDetailsViewModel;
        private readonly IDivisionViewModelMapper _divisionViewModelMapper;
        private readonly IDivisionRepository _divisionRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionController"/> class.
        /// </summary>
        /// <param name="divisionIndexViewModel">
        /// The <see cref="IDivisionIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="divisionDetailsViewModel">
        /// The <see cref="IDivisionsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="divisionViewModelMapper">
        /// The <see cref="IDivisionViewModelMapper"/> by which division data will be mapped to view models.
        /// </param>
        /// <param name="divisionRepository">
        /// The <see cref="IDivisionRepository"/> by which division data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public DivisionController(
            IDivisionIndexViewModel divisionIndexViewModel,
            IDivisionDetailsViewModel divisionDetailsViewModel,
            IDivisionViewModelMapper divisionViewModelMapper,
            IDivisionRepository divisionRepository,
            ISharedRepository sharedRepository
        )
        {
            _divisionIndexViewModel = divisionIndexViewModel;
            _divisionDetailsViewModel = divisionDetailsViewModel;
            _divisionViewModelMapper = divisionViewModelMapper;
            _divisionRepository = divisionRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Divisions
        /// <summary>
        /// Renders a view of the Divisions list.
        /// </summary>
        /// <returns>The rendered view of the Divisions list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var divisions = await _divisionRepository.GetDivisionsAsync();
            _divisionIndexViewModel.Divisions = divisions
                .Select(d => _divisionViewModelMapper.MapDivisionToViewModel(d))
                .OrderBy(d => d.Name);

            return View(_divisionIndexViewModel);
        }

        // GET: Divisions/Details/5
        /// <summary>
        /// Renders a view of the details of a selected division.
        /// </summary>
        /// <param name="id">The Id of the selected division.</param>
        /// <returns>The rendered view of the selected division.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var division = await _divisionRepository.GetDivisionAsync(id.Value);
            if (division is null)
            {
                return NotFound();
            }

            _divisionDetailsViewModel.Division = _divisionViewModelMapper.MapDivisionToViewModel(division);

            return View(_divisionDetailsViewModel);
        }

        // GET: Divisions/Create
        /// <summary>
        /// Renders a view of the division create form.
        /// </summary>
        /// <returns>The rendered view of the division create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Divisions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the division create form.
        /// </summary>
        /// <param name="divisionViewModel">A <see cref="Division"/> object with the data provided for the new division.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,LeagueName,ConferenceName,FirstSeasonYear,LastSeasonYear")] DivisionViewModel divisionViewModel)
        {
            if (ModelState.IsValid)
            {
                var division = await _divisionViewModelMapper.MapViewModelToDivision(divisionViewModel);
                await _divisionRepository.AddAsync(division);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, division);
                    return View(divisionViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(divisionViewModel);
        }

        // GET: Divisions/Edit/5
        /// <summary>
        /// Renders a view of the division edit form.
        /// </summary>
        /// <returns>The rendered view of the division edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var division = await _divisionRepository.GetDivisionAsync(id.Value);
            if (division is null)
            {
                return NotFound();
            }

            var divisionViewModel = new DivisionViewModel { Division = division };
            return View(divisionViewModel);
        }

        // POST: Divisions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the division edit form.
        /// </summary>
        /// <param name="divisionViewModel">A <see cref="Division"/> object with the data provided for the division game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LeagueName,ConferenceName,FirstSeasonYear,LastSeasonYear")] DivisionViewModel divisionViewModel)
        {
            if (id != divisionViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var division = await _divisionViewModelMapper.MapViewModelToDivision(divisionViewModel);
                _divisionRepository.Update(division);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _divisionRepository.DivisionExistsAsync(division.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex, division);
                    return View(divisionViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(divisionViewModel);
        }

        // GET: Divisions/Delete/5
        /// <summary>
        /// Renders a view of the division delete form.
        /// </summary>
        /// <returns>The rendered view of the division delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var division = await _divisionRepository.GetDivisionAsync(id.Value);
            if (division is null)
            {
                return NotFound();
            }

            var divisionViewModel = _divisionViewModelMapper.MapDivisionToViewModel(division);
            return View(divisionViewModel);
        }

        // POST: Divisions/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a division.
        /// </summary>
        /// <param name="id">The Id of the division to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _divisionRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Division division)
        {
            var divisions = await _divisionRepository.GetDivisionsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(divisions, division))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A division with the same Id already exists.");
            }
            else if (UniqueKeyViolationExistsOnCreate(divisions, division))
            {
                ModelState.AddModelError("Name", $"{errMsgIntro} A division with the same name already exists.");
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

        private bool PrimaryKeyViolationExists(IEnumerable<Division> divisions, Division division)
        {
            return divisions.Any(d => d.Id == division.Id);
        }

        private bool UniqueKeyViolationExistsOnCreate(IEnumerable<Division> divisions, Division division)
        {
            return divisions.Any(d => d.Name == division.Name);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, Division division)
        {
            var divisions = await _divisionRepository.GetDivisionsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyViolationExistsOnEdit(divisions, division))
            {
                ModelState.AddModelError("Name", $"{errMsgIntro} A division with the same name already exists.");
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

        private bool UniqueKeyViolationExistsOnEdit(IEnumerable<Division> divisions, Division division)
        {
            return divisions.Count(d => d.Name == division.Name) > 1;
        }
    }
}
