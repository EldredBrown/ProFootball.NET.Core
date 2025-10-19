using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of division data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DivisionsController : Controller
    {
        private readonly IDivisionsIndexViewModel _divisionsIndexViewModel;
        private readonly IDivisionsDetailsViewModel _divisionsDetailsViewModel;
        private readonly IDivisionRepository _divisionRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionsController"/> class.
        /// </summary>
        /// <param name="divisionsIndexViewModel">
        /// The <see cref="IDivisionsIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="divisionsDetailsViewModel">
        /// The <see cref="IDivisionsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="divisionRepository">
        /// The <see cref="IDivisionRepository"/> by which division data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public DivisionsController(
            IDivisionsIndexViewModel divisionsIndexViewModel,
            IDivisionsDetailsViewModel divisionsDetailsViewModel,
            IDivisionRepository divisionRepository,
            ISharedRepository sharedRepository)
        {
            _divisionsIndexViewModel = divisionsIndexViewModel;
            _divisionsDetailsViewModel = divisionsDetailsViewModel;
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
            _divisionsIndexViewModel.Divisions = await _divisionRepository.GetDivisionsAsync();

            return View(_divisionsIndexViewModel);
        }

        // GET: Divisions/Details/5
        /// <summary>
        /// Renders a view of the details of a selected division.
        /// </summary>
        /// <param name="id">The ID of the selected division.</param>
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

            _divisionsDetailsViewModel.Division = division;

            return View(_divisionsDetailsViewModel);
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
        /// <param name="division">A <see cref="Division"/> object with the data provided for the new division.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LongName,ShortName,FirstSeasonYear,LastSeasonYear")] Division division)
        {
            if (ModelState.IsValid)
            {
                await _divisionRepository.AddAsync(division);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(division);
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

            return View(division);
        }

        // POST: Divisions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the division edit form.
        /// </summary>
        /// <param name="division">A <see cref="Division"/> object with the data provided for the division game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LongName,ShortName,FirstSeasonYear,LastSeasonYear")] Division division)
        {
            if (id != division.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _divisionRepository.Update(division);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _divisionRepository.DivisionExists(division.ID)))
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

            return View(division);
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

            return View(division);
        }

        // POST: Divisions/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a division.
        /// </summary>
        /// <param name="id">The ID of the division to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _divisionRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
