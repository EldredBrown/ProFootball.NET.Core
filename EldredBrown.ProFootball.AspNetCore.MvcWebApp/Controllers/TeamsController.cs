using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of team data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private readonly ITeamsIndexViewModel _teamsIndexViewModel;
        private readonly ITeamsDetailsViewModel _teamsDetailsViewModel;
        private readonly ITeamRepository _teamRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsController"/> class.
        /// </summary>
        /// <param name="teamsIndexViewModel">
        /// The <see cref="ITeamsIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="teamsDetailsViewModel">
        /// The <see cref="ITeamsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="teamRepository">
        /// The <see cref="ITeamRepository"/> by which team data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public TeamsController(
            ITeamsIndexViewModel teamsIndexViewModel,
            ITeamsDetailsViewModel teamsDetailsViewModel,
            ITeamRepository teamRepository,
            ISharedRepository sharedRepository)
        {
            _teamsIndexViewModel = teamsIndexViewModel;
            _teamsDetailsViewModel = teamsDetailsViewModel;
            _teamRepository = teamRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Teams
        /// <summary>
        /// Renders a view of the Teams list.
        /// </summary>
        /// <returns>The rendered view of the Teams list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _teamsIndexViewModel.Teams = await _teamRepository.GetTeamsAsync();

            return View(_teamsIndexViewModel);
        }

        // GET: Teams/Details/5
        /// <summary>
        /// Renders a view of the details of a selected team.
        /// </summary>
        /// <param name="id">The ID of the selected team.</param>
        /// <returns>The rendered view of the selected team.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetTeamAsync(id.Value);
            if (team is null)
            {
                return NotFound();
            }

            _teamsDetailsViewModel.Team = team;

            return View(_teamsDetailsViewModel);
        }

        // GET: Teams/Create
        /// <summary>
        /// Renders a view of the team create form.
        /// </summary>
        /// <returns>The rendered view of the team create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the team create form.
        /// </summary>
        /// <param name="team">A <see cref="Team"/> object with the data provided for the new team.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Team team)
        {
            if (ModelState.IsValid)
            {
                await _teamRepository.AddAsync(team);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(team);
        }

        // GET: Teams/Edit/5
        /// <summary>
        /// Renders a view of the team edit form.
        /// </summary>
        /// <returns>The rendered view of the team edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetTeamAsync(id.Value);
            if (team is null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the team edit form.
        /// </summary>
        /// <param name="team">A <see cref="Team"/> object with the data provided for the team game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _teamRepository.Update(team);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _teamRepository.TeamExists(team.ID))
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

            return View(team);
        }

        // GET: Teams/Delete/5
        /// <summary>
        /// Renders a view of the team delete form.
        /// </summary>
        /// <returns>The rendered view of the team delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetTeamAsync(id.Value);
            if (team is null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a team.
        /// </summary>
        /// <param name="id">The ID of the team to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
