using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of team data.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    public class TeamController : Controller
    {
        private readonly ITeamIndexViewModel _teamIndexViewModel;
        private readonly ITeamDetailsViewModel _teamDetailsViewModel;
        private readonly ITeamRepository _teamRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class.
        /// </summary>
        /// <param name="teamIndexViewModel">
        /// The <see cref="ITeamIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="teamDetailsViewModel">
        /// The <see cref="ITeamsDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="teamRepository">
        /// The <see cref="ITeamRepository"/> by which team data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public TeamController(
            ITeamIndexViewModel teamIndexViewModel,
            ITeamDetailsViewModel teamDetailsViewModel,
            ITeamRepository teamRepository,
            ISharedRepository sharedRepository
        )
        {
            _teamIndexViewModel = teamIndexViewModel;
            _teamDetailsViewModel = teamDetailsViewModel;
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
            var teams = await _teamRepository.GetTeamsAsync();
            _teamIndexViewModel.Teams = teams.OrderBy(t => t.Name);
            return View(_teamIndexViewModel);
        }

        // GET: Teams/Details/5
        /// <summary>
        /// Renders a view of the details of a selected team.
        /// </summary>
        /// <param name="id">The Id of the selected team.</param>
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

            _teamDetailsViewModel.Team = team;

            return View(_teamDetailsViewModel);
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

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, team);
                    return View(team);
                }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _teamRepository.Update(team);

                try
                {
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _teamRepository.TeamExistsAsync(team.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex, team);
                    return View(team);
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
        /// <param name="id">The Id of the team to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Team team)
        {
            var teams = await _teamRepository.GetTeamsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(teams, team))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A team with the same Id already exists.");
            }
            else if (UniqueKeyViolationExistsOnCreate(teams, team))
            {
                ModelState.AddModelError("Name", $"{errMsgIntro} A team with the same name already exists.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} An unexpected error occurred.");
            }
        }

        private bool PrimaryKeyViolationExists(IEnumerable<Team> teams, Team team)
        {
            return teams.Any(t => t.Id == team.Id);
        }

        private bool UniqueKeyViolationExistsOnCreate(IEnumerable<Team> teams, Team team)
        {
            return teams.Any(t => t.Name == team.Name);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, Team team)
        {
            var teams = await _teamRepository.GetTeamsAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyViolationExistsOnEdit(teams, team))
            {
                ModelState.AddModelError("Name", $"{errMsgIntro} A team with the same name already exists.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{errMsgIntro} An unexpected error occurred.");
            }
        }

        private bool UniqueKeyViolationExistsOnEdit(IEnumerable<Team> teams, Team team)
        {
            return teams.Count(t => t.Name == team.Name) > 1;
        }
    }
}
