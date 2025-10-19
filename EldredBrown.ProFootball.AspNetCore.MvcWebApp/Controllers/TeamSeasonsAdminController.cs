using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of team season data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class TeamSeasonsAdminController : Controller
    {
        private readonly ITeamSeasonsIndexViewModel _teamSeasonsIndexViewModel;
        private readonly ITeamSeasonsDetailsViewModel _teamSeasonsDetailsViewModel;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonsAdminController"/> class.
        /// </summary>
        /// <param name="teamSeasonsIndexViewModel">
        /// The <see cref="ITeamSeasonsIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="teamSeasonsDetailsViewModel">
        /// The <see cref="ITeamSeasonsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="teamSeasonRepository">
        /// The <see cref="ITeamSeasonRepository"/> by which league data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public TeamSeasonsAdminController(
            ITeamSeasonsIndexViewModel teamSeasonsIndexViewModel,
            ITeamSeasonsDetailsViewModel teamSeasonsDetailsViewModel,
            ITeamSeasonRepository teamSeasonRepository,
            ISharedRepository sharedRepository)
        {
            _teamSeasonsIndexViewModel = teamSeasonsIndexViewModel;
            _teamSeasonsDetailsViewModel = teamSeasonsDetailsViewModel;
            _teamSeasonRepository = teamSeasonRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: TeamSeasons
        /// <summary>
        /// Renders a view of the TeamSeasons list.
        /// </summary>
        /// <returns>The rendered view of the TeamSeasons list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _teamSeasonsIndexViewModel.TeamSeasons = await _teamSeasonRepository.GetTeamSeasonsAsync();

            return View(_teamSeasonsIndexViewModel);
        }

        // GET: TeamSeasons/Details/5
        /// <summary>
        /// Renders a view of the details of a selected team season.
        /// </summary>
        /// <param name="id">The ID of the selected team season.</param>
        /// <returns>The rendered view of the selected team season.</returns>
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

            _teamSeasonsDetailsViewModel.TeamSeason = teamSeason;

            return View(_teamSeasonsDetailsViewModel);
        }

        // GET: TeamSeasons/Create
        /// <summary>
        /// Renders a view of the team season create form.
        /// </summary>
        /// <returns>The rendered view of the team season create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TeamSeasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the team season create form.
        /// </summary>
        /// <param name="teamSeason">A <see cref="TeamSeason"/> object with the data provided for the new team season.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamName,SeasonYear,LeagueName")] TeamSeason teamSeason)
        {
            if (ModelState.IsValid)
            {
                await _teamSeasonRepository.AddAsync(teamSeason);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(teamSeason);
        }

        // GET: TeamSeasons/Edit/5
        /// <summary>
        /// Renders a view of the team season edit form.
        /// </summary>
        /// <returns>The rendered view of the team season edit form.</returns>
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

            return View(teamSeason);
        }

        // POST: TeamSeasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the team season edit form.
        /// </summary>
        /// <param name="teamSeason">A <see cref="TeamSeason"/> object with the data provided for the team season game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TeamName,SeasonYear,LeagueName")] TeamSeason teamSeason)
        {
            if (id != teamSeason.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _teamSeasonRepository.Update(teamSeason);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _teamSeasonRepository.TeamSeasonExists(teamSeason.ID))
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

            return View(teamSeason);
        }

        // GET: TeamSeasons/Delete/5
        /// <summary>
        /// Renders a view of the team season delete form.
        /// </summary>
        /// <returns>The rendered view of the team season delete form.</returns>
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

            return View(teamSeason);
        }

        // POST: TeamSeasons/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a team season.
        /// </summary>
        /// <param name="id">The ID of the team season to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamSeasonRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
