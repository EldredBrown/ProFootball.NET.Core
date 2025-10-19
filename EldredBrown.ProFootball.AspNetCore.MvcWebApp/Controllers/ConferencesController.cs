using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of conference data.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ConferencesController : Controller
    {
        private readonly IConferencesIndexViewModel _conferencesIndexViewModel;
        private readonly IConferencesDetailsViewModel _conferencesDetailsViewModel;
        private readonly IConferenceRepository _conferenceRepository;
        private readonly ISharedRepository _sharedRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferencesController"/> class.
        /// </summary>
        /// <param name="conferencesIndexViewModel">
        /// The <see cref="IConferencesIndexViewModel"/> that will provide ViewModel data to the Index view.
        /// </param>
        /// <param name="conferencesDetailsViewModel">
        /// The <see cref="IConferencesDetailsViewModel"/> that will provide ViewModel data to the Details view.
        /// </param>
        /// <param name="conferenceRepository">
        /// The <see cref="IConferenceRepository"/> by which conference data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        public ConferencesController(
            IConferencesIndexViewModel conferencesIndexViewModel,
            IConferencesDetailsViewModel conferencesDetailsViewModel,
            IConferenceRepository conferenceRepository,
            ISharedRepository sharedRepository)
        {
            _conferencesIndexViewModel = conferencesIndexViewModel;
            _conferencesDetailsViewModel = conferencesDetailsViewModel;
            _conferenceRepository = conferenceRepository;
            _sharedRepository = sharedRepository;
        }

        // GET: Conferences
        /// <summary>
        /// Renders a view of the Conferences list.
        /// </summary>
        /// <returns>The rendered view of the Conferences list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _conferencesIndexViewModel.Conferences = await _conferenceRepository.GetConferencesAsync();

            return View(_conferencesIndexViewModel);
        }

        // GET: Conferences/Details/5
        /// <summary>
        /// Renders a view of the details of a selected conference.
        /// </summary>
        /// <param name="id">The ID of the selected conference.</param>
        /// <returns>The rendered view of the selected conference.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var conference = await _conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            _conferencesDetailsViewModel.Conference = conference;

            return View(_conferencesDetailsViewModel);
        }

        // GET: Conferences/Create
        /// <summary>
        /// Renders a view of the conference create form.
        /// </summary>
        /// <returns>The rendered view of the conference create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conferences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the conference create form.
        /// </summary>
        /// <param name="conference">A <see cref="Conference"/> object with the data provided for the new conference.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LongName,ShortName,FirstSeasonYear,LastSeasonYear")] Conference conference)
        {
            if (ModelState.IsValid)
            {
                await _conferenceRepository.AddAsync(conference);
                await _sharedRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(conference);
        }

        // GET: Conferences/Edit/5
        /// <summary>
        /// Renders a view of the conference edit form.
        /// </summary>
        /// <returns>The rendered view of the conference edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var conference = await _conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            return View(conference);
        }

        // POST: Conferences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the conference edit form.
        /// </summary>
        /// <param name="conference">A <see cref="Conference"/> object with the data provided for the conference game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LongName,ShortName,FirstSeasonYear,LastSeasonYear")] Conference conference)
        {
            if (id != conference.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _conferenceRepository.Update(conference);
                    await _sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _conferenceRepository.ConferenceExists(conference.ID)))
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

            return View(conference);
        }

        // GET: Conferences/Delete/5
        /// <summary>
        /// Renders a view of the conference delete form.
        /// </summary>
        /// <returns>The rendered view of the conference delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var conference = await _conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            return View(conference);
        }

        // POST: Conferences/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a conference.
        /// </summary>
        /// <param name="id">The ID of the conference to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _conferenceRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
