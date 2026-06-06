using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using System;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of season data.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    public class SeasonController : Controller
    {
        private readonly ISeasonIndexViewModel _seasonIndexViewModel;
        private readonly ISeasonDetailsViewModel _seasonDetailsViewModel;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonController"/> class.
        /// </summary>
        /// <param name="seasonIndexViewModel">
        /// The <see cref="ISeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="seasonDetailsViewModel">
        /// The <see cref="ISeasonDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public SeasonController(
            ISeasonIndexViewModel seasonIndexViewModel,
            ISeasonDetailsViewModel seasonDetailsViewModel,
            ISeasonRepository seasonRepository,
            ISharedRepository sharedRepository
        )
        {
            _seasonIndexViewModel = seasonIndexViewModel;
            _seasonDetailsViewModel = seasonDetailsViewModel;
            _seasonRepository = seasonRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Seasons
        /// <summary>
        /// Renders a view of the Seasons list.
        /// </summary>
        /// <returns>The rendered view of the Seasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _seasonIndexViewModel.Seasons = await _seasonRepository.GetSeasonsAsync();

            return View(_seasonIndexViewModel);
        }

        // GET: Seasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected season.
        /// </summary>
        /// <param name="year">The year of the selected season.</param>
        /// <returns>The rendered view of the selected season.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            _seasonDetailsViewModel.Title = "Season";

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
            if (season is null)
            {
                return NotFound();
            }
            _seasonDetailsViewModel.Season = season;

            return View(_seasonDetailsViewModel);
        }

        // GET: Seasons/Create
        /// <summary>
        /// Renders a view of the season create form.
        /// </summary>
        /// <returns>The rendered view of the season create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Seasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the season create form.
        /// </summary>
        /// <param name="season">A <see cref="Season"/> object with the data provided for the new season.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumOfWeeksScheduled,NumOfWeeksCompleted")] Season season)
        {
            if (ModelState.IsValid)
            {
                await _seasonRepository.AddAsync(season);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, season);
                    return View(season);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(season);
        }

        // GET: Seasons/Edit/5
        /// <summary>
        /// Renders a view of the season edit form.
        /// </summary>
        /// <returns>The rendered view of the season edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
            if (season is null)
            {
                return NotFound();
            }

            return View(season);
        }

        // POST: Seasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the season edit form.
        /// </summary>
        /// <param name="season">A <see cref="Season"/> object with the data provided for the season game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumOfWeeksScheduled,NumOfWeeksCompleted")] Season season)
        {
            if (id != season.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _seasonRepository.Update(season);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _seasonRepository.SeasonExistsAsync(season.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes. An unexpected error occurred.");
                    return View(season);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(season);
        }

        // GET: Seasons/Delete/5
        /// <summary>
        /// Renders a view of the season delete form.
        /// </summary>
        /// <returns>The rendered view of the season delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
            if (season is null)
            {
                return NotFound();
            }

            return View(season);
        }

        // POST: Seasons/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a season.
        /// </summary>
        /// <param name="id">The Id of the season to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _seasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Season season)
        {
            var seasons = await _seasonRepository.GetSeasonsAsync();
            var intro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(seasons, season))
            {
                ModelState.AddModelError("Id", $"{intro} A season with the same id already exists.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{intro} An unexpected error occurred.");
            }
        }

        private bool PrimaryKeyViolationExists(IEnumerable<Season> seasons, Season season)
        {
            return seasons.Any(s => s.Id == season.Id);
        }
    }
}
