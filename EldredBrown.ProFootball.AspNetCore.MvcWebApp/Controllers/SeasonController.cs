using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of season data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonController"/> class.
    /// </remarks>
    /// <param name="_seasonIndexViewModel">
    /// The <see cref="ISeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="_seasonDetailsViewModel">
    /// The <see cref="ISeasonDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="_seasonRepository">
    /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
    /// </param>
    /// <param name="_sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class SeasonController(
        ISeasonIndexViewModel seasonIndexViewModel,
        ISeasonDetailsViewModel seasonDetailsViewModel,
        ISeasonRepository seasonRepository,
        ISharedRepository sharedRepository
    ) : Controller
    {
        internal readonly ISeasonIndexViewModel _seasonIndexViewModel = seasonIndexViewModel;
        internal readonly ISeasonDetailsViewModel _seasonDetailsViewModel = seasonDetailsViewModel;
        internal readonly ISeasonRepository _seasonRepository = seasonRepository;
        internal readonly ISharedRepository _sharedRepository = sharedRepository;

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
        public async Task<IActionResult> Details(int? year)
        {
            if (year is null)
            {
                return NotFound();
            }

            _seasonDetailsViewModel.Title = "Season";

            var season = await _seasonRepository.GetSeasonAsync(year.Value);
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
        public async Task<IActionResult> Create([Bind("Year")] Season season)
        {
            if (ModelState.IsValid)
            {
                await _seasonRepository.AddAsync(season);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    await HandleDbUpdateExceptionOnCreate(season);
                    return View(season);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(season);
        }

        // Unlike most of my controllers, this controller does not have an Edit action because the only property of a
        // Season is the Year, which is the primary key. Therefore, there is no way to edit a Season.

        // GET: Seasons/Delete/5
        /// <summary>
        /// Renders a view of the season delete form.
        /// </summary>
        /// <returns>The rendered view of the season delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? year)
        {
            if (year is null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAsync(year.Value);
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
        /// <param name="year">The Id of the season to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int year)
        {
            var season = await _seasonRepository.DeleteAsync(year);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(Season season)
        {
            var seasons = await _seasonRepository.GetSeasonsAsync();

            if (PrimaryKeyViolationExists(seasons, season))
            {
                ModelState.AddModelError("Year", $"{DbVerificationUtils.ErrMsgIntro} A season with the same year already exists.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{DbVerificationUtils.ErrMsgIntro} An unexpected error occurred.");
            }
        }

        private static bool PrimaryKeyViolationExists(IEnumerable<Season> seasons, Season season)
        {
            return seasons.Any(s => s.Year == season.Year);
        }
    }
}
