using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of season data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class SeasonsController : Controller
    {
        private readonly ISeasonsIndexViewModel _seasonsIndexViewModel;
        private readonly ISeasonsDetailsViewModel _seasonsDetailsViewModel;
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonsController"/> class.
        /// </summary>
        /// <param name="seasonsIndexViewModel">
        /// The <see cref="ISeasonsIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="seasonsDetailsViewModel">
        /// The <see cref="ISeasonsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> by which season data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public SeasonsController(
            ISeasonsIndexViewModel seasonsIndexViewModel,
            ISeasonsDetailsViewModel seasonsDetailsViewModel,
            ISeasonRepository seasonRepository,
            ISharedRepository sharedRepository
        )
        {
            _seasonsIndexViewModel = seasonsIndexViewModel;
            _seasonsDetailsViewModel = seasonsDetailsViewModel;
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
            _seasonsIndexViewModel.Seasons = await _seasonRepository.GetSeasonsAsync();

            return View(_seasonsIndexViewModel);
        }

        // GET: Seasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected season.
        /// </summary>
        /// <param name="id">The ID of the selected season.</param>
        /// <returns>The rendered view of the selected season.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            _seasonsDetailsViewModel.Title = "Season";

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
            if (season is null)
            {
                return NotFound();
            }
            _seasonsDetailsViewModel.Season = season;

            return View(_seasonsDetailsViewModel);
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
        public async Task<IActionResult> Create([Bind("Year,NumOfWeeksScheduled,NumOfWeeksCompleted")] Season season)
        {
            if (ModelState.IsValid)
            {
                await _seasonRepository.AddAsync(season);
                await _sharedRepository.SaveChangesAsync();

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Year,NumOfWeeksScheduled,NumOfWeeksCompleted")] Season season)
        {
            if (id != season.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _seasonRepository.Update(season);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _seasonRepository.SeasonExists(season.ID))
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
        /// <param name="id">The ID of the season to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _seasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
