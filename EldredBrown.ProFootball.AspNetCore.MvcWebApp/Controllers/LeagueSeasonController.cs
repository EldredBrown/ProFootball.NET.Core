using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of league season data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class LeagueSeasonController : Controller
    {
        private readonly ILeagueSeasonIndexViewModel _leagueSeasonIndexViewModel;
        private readonly ILeagueSeasonDetailsViewModel _leagueSeasonDetailsViewModel;
        private readonly ILeagueSeasonRepository _leagueSeasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueSeasonController"/> class.
        /// </summary>
        /// <param name="leagueSeasonIndexViewModel">
        /// The <see cref="ILeagueSeasonIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="leagueSeasonDetailsViewModel">
        /// The <see cref="ILeagueSeasonDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="leagueSeasonRepository">
        /// The <see cref="ILeagueSeasonRepository"/> by which leagueSeason data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public LeagueSeasonController(
            ILeagueSeasonIndexViewModel leagueSeasonIndexViewModel,
            ILeagueSeasonDetailsViewModel leagueSeasonDetailsViewModel,
            ILeagueSeasonRepository leagueSeasonRepository,
            ISharedRepository sharedRepository)
        {
            _leagueSeasonIndexViewModel = leagueSeasonIndexViewModel;
            _leagueSeasonDetailsViewModel = leagueSeasonDetailsViewModel;
            _leagueSeasonRepository = leagueSeasonRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: LeagueSeasons
        /// <summary>
        /// Renders a view of the LeagueSeasons list.
        /// </summary>
        /// <returns>The rendered view of the LeagueSeasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _leagueSeasonIndexViewModel.LeagueSeasons = await _leagueSeasonRepository.GetLeagueSeasonsAsync();

            return View(_leagueSeasonIndexViewModel);
        }

        // GET: LeagueSeasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected league season.
        /// </summary>
        /// <param name="id">The Id of the selected league season.</param>
        /// <returns>The rendered view of the selected league season.</returns>
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

            _leagueSeasonDetailsViewModel.LeagueSeason = leagueSeason;

            return View(_leagueSeasonDetailsViewModel);
        }

        // GET: LeagueSeasons/Create
        /// <summary>
        /// Renders a view of the league season create form.
        /// </summary>
        /// <returns>The rendered view of the league season create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeagueSeasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the league season create form.
        /// </summary>
        /// <param name="leagueSeason">A <see cref="LeagueSeason"/> object with the data provided for the new league season.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeagueName,SeasonYear,TotalGames,TotalPoints,AveragePoints")] LeagueSeason leagueSeason)
        {
            if (ModelState.IsValid)
            {
                await _leagueSeasonRepository.AddAsync(leagueSeason);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(leagueSeason);
        }

        // GET: LeagueSeasons/Edit/5
        /// <summary>
        /// Renders a view of the league season edit form.
        /// </summary>
        /// <returns>The rendered view of the league season edit form.</returns>
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

            return View(leagueSeason);
        }

        // POST: LeagueSeasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the league season edit form.
        /// </summary>
        /// <param name="leagueSeason">A <see cref="LeagueSeason"/> object with the data provided for the league season game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LeagueName,SeasonYear,TotalGames,TotalPoints,AveragePoints")] LeagueSeason leagueSeason)
        {
            if (id != leagueSeason.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _leagueSeasonRepository.Update(leagueSeason);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _leagueSeasonRepository.LeagueSeasonExists(leagueSeason.Id))
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

            return View(leagueSeason);
        }

        // GET: LeagueSeasons/Delete/5
        /// <summary>
        /// Renders a view of the league season delete form.
        /// </summary>
        /// <returns>The rendered view of the league season delete form.</returns>
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

            return View(leagueSeason);
        }

        // POST: LeagueSeasons/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a league season.
        /// </summary>
        /// <param name="id">The Id of the league season to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _leagueSeasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
