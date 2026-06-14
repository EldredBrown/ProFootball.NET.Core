using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of conference data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConferenceController"/> class.
    /// </remarks>
    /// <param name="conferenceIndexViewModel">
    /// The <see cref="IConferenceIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="conferenceDetailsViewModel">
    /// The <see cref="IConferencesDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="conferenceViewModelMapper">
    /// The <see cref="IConferenceViewModelMapper"/> by which conference data will be mapped to view models.
    /// </param>
    /// <param name="conferenceRepository">
    /// The <see cref="IConferenceRepository"/> by which conference data will be accessed.
    /// </param>
    /// <param name="sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class ConferenceController(
        IConferenceIndexViewModel conferenceIndexViewModel,
        IConferenceDetailsViewModel conferenceDetailsViewModel,
        IConferenceViewModelMapper conferenceViewModelMapper,
        IConferenceRepository conferenceRepository,
        ISharedRepository sharedRepository
        ) : Controller
    {
        // GET: Conferences
        /// <summary>
        /// Renders a view of the Conferences list.
        /// </summary>
        /// <returns>The rendered view of the Conferences list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var conferences = await conferenceRepository.GetConferencesAsync();
            conferenceIndexViewModel.Conferences = conferences
                .Select(c => conferenceViewModelMapper.MapConferenceToViewModel(c))
                .OrderBy(c => c.ShortName)
                .ToList();

            return View(conferenceIndexViewModel);
        }

        // GET: Conferences/Details/5
        /// <summary>
        /// Renders a view of the details of a selected conference.
        /// </summary>
        /// <param name="id">The Id of the selected conference.</param>
        /// <returns>The rendered view of the selected conference.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var conference = await conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            conferenceDetailsViewModel.Conference = conferenceViewModelMapper.MapConferenceToViewModel(conference);

            return View(conferenceDetailsViewModel);
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
        /// <param name="conferenceViewModel">A <see cref="Conference"/> object with the data provided for the new conference.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortName,LongName,LeagueName,FirstSeasonYear,LastSeasonYear")] ConferenceViewModel conferenceViewModel)
        {
            if (ModelState.IsValid)
            {
                var conference = await conferenceViewModelMapper.MapViewModelToConference(conferenceViewModel);
                await conferenceRepository.AddAsync(conference);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, conference);
                    return View(conferenceViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(conferenceViewModel);
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

            var conference = await conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            return View(conferenceViewModel);
        }

        // POST: Conferences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the conference edit form.
        /// </summary>
        /// <param name="conferenceViewModel">A <see cref="Conference"/> object with the data provided for the conference game.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShortName,LongName,LeagueName,FirstSeasonYear,LastSeasonYear")] ConferenceViewModel conferenceViewModel)
        {
            if (id != conferenceViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var conference = await conferenceViewModelMapper.MapViewModelToConference(conferenceViewModel);
                conferenceRepository.Update(conference);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await conferenceRepository.ConferenceExistsAsync(conference.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex, conference);
                    return View(conferenceViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(conferenceViewModel);
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

            var conference = await conferenceRepository.GetConferenceAsync(id.Value);
            if (conference is null)
            {
                return NotFound();
            }

            var conferenceViewModel = conferenceViewModelMapper.MapConferenceToViewModel(conference);
            return View(conferenceViewModel);
        }

        // POST: Conferences/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete a conference.
        /// </summary>
        /// <param name="id">The Id of the conference to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await conferenceRepository.DeleteAsync(id);
            await sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Conference conference)
        {
            var conferences = await conferenceRepository.GetConferencesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (PrimaryKeyViolationExists(conferences, conference))
            {
                ModelState.AddModelError("Id", $"{errMsgIntro} A conference with the same Id already exists.");
            }
            else if (UniqueKeyShortNameViolationExistsOnCreate(conferences, conference))
            {
                ModelState.AddModelError("ShortName", $"{errMsgIntro} A conference with the same short name already exists.");
            }
            else if (UniqueKeyLongNameViolationExistsOnCreate(conferences, conference))
            {
                ModelState.AddModelError("LongName", $"{errMsgIntro} A conference with the same long name already exists.");
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

        private bool PrimaryKeyViolationExists(IEnumerable<Conference> conferences, Conference conference)
        {
            return conferences.Any(c => c.Id == conference.Id);
        }

        private bool UniqueKeyShortNameViolationExistsOnCreate(IEnumerable<Conference> conferences, Conference conference)
        {
            return conferences.Any(c => c.ShortName == conference.ShortName);
        }

        private bool UniqueKeyLongNameViolationExistsOnCreate(IEnumerable<Conference> conferences, Conference conference)
        {
            return conferences.Any(c => c.LongName == conference.LongName);
        }

        private async Task HandleDbUpdateExceptionOnEdit(DbUpdateException ex, Conference conference)
        {
            var conferences = await conferenceRepository.GetConferencesAsync();
            var errMsgIntro = "Unable to save changes.";

            if (UniqueKeyShortNameViolationExistsOnEdit(conferences, conference))
            {
                ModelState.AddModelError("ShortName", $"{errMsgIntro} A conference with the same short name already exists.");
            }
            else if (UniqueKeyLongNameViolationExistsOnEdit(conferences, conference))
            {
                ModelState.AddModelError("LongName", $"{errMsgIntro} A conference with the same long name already exists.");
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

        private bool UniqueKeyShortNameViolationExistsOnEdit(IEnumerable<Conference> conferences, Conference conference)
        {
            return conferences.Count(c => c.ShortName == conference.ShortName) > 1;
        }

        private bool UniqueKeyLongNameViolationExistsOnEdit(IEnumerable<Conference> conferences, Conference conference)
        {
            return conferences.Count(c => c.LongName == conference.LongName) > 1;
        }
    }
}
