using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    /// <summary>
    /// Provides control of the flow of execution for views of association data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AssociationController"/> class.
    /// </remarks>
    /// <param name="associationIndexViewModel">
    /// The <see cref="IAssociationIndexViewModel"/> that will provide ViewModel data to the Index view.
    /// </param>
    /// <param name="associationDetailsViewModel">
    /// The <see cref="IAssociationDetailsViewModel"/> that will provide ViewModel data to the Details view.
    /// </param>
    /// <param name="associationViewModelMapper">
    /// The <see cref="IAssociationViewModelMapper"/> by which association data will be mapped to view models.
    /// </param>
    /// <param name="associationRepository">
    /// The <see cref="IAssociationRepository"/> by which association data will be accessed.
    /// </param>
    /// <param name="sharedRepository">
    /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
    /// </param>
    //[Authorize(Roles = "Admin")]
    public class AssociationController(
        IAssociationIndexViewModel associationIndexViewModel,
        IAssociationDetailsViewModel associationDetailsViewModel,
        IAssociationViewModelMapper associationViewModelMapper,
        IAssociationRepository associationRepository,
        ISharedRepository sharedRepository
    ) : Controller
    {
        // GET: Associations
        /// <summary>
        /// Renders a view of the Associations list.
        /// </summary>
        /// <returns>The rendered view of the Associations list.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var associations = await associationRepository.GetAssociationsAsync();
            associationIndexViewModel.Associations = [.. associations
                .Select(a => associationViewModelMapper.MapAssociationToViewModel(a))
                .OrderBy(a => a.ParentName)];

            return View(associationIndexViewModel);
        }

        // GET: Associations/Details/5
        /// <summary>
        /// Renders a view of the details of a selected association.
        /// </summary>
        /// <param name="id">The Id of the selected association.</param>
        /// <returns>The rendered view of the selected association.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var association = await associationRepository.GetAssociationAsync(id.Value);
            if (association is null)
            {
                return NotFound();
            }

            associationDetailsViewModel.Association = associationViewModelMapper.MapAssociationToViewModel(association);

            return View(associationDetailsViewModel);
        }

        // GET: Associations/Create
        /// <summary>
        /// Renders a view of the association create form.
        /// </summary>
        /// <returns>The rendered view of the association create form.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Associations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the association create form.
        /// </summary>
        /// <param name="associationViewModel">A <see cref="Association"/> object with the data provided for the new association.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LongName,ShortName,ParentName,FirstSeasonYear,LastSeasonYear")] AssociationViewModel associationViewModel
        )
        {
            if (ModelState.IsValid)
            {
                var association = await associationViewModelMapper.MapViewModelToAssociation(associationViewModel);
                await associationRepository.AddAsync(association);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await HandleDbUpdateExceptionOnCreate(ex, association);
                    return View(associationViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(associationViewModel);
        }

        // GET: Associations/Edit/5
        /// <summary>
        /// Renders a view of the association edit form.
        /// </summary>
        /// <returns>The rendered view of the association edit form.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var association = await associationRepository.GetAssociationAsync(id.Value);
            if (association is null)
            {
                return NotFound();
            }

            var associationViewModel = new AssociationViewModel { Association = association };
            return View(associationViewModel);
        }

        // POST: Associations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Processes the data posted back from the association edit form.
        /// </summary>
        /// <param name="associationViewModel">A <see cref="Association"/> object with the data provided for the association.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,LongName,ShortName,ParentName,FirstSeasonYear,LastSeasonYear")] AssociationViewModel associationViewModel
        )
        {
            if (id != associationViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var association = await associationViewModelMapper.MapViewModelToAssociation(associationViewModel);
                associationRepository.Update(association);

                try
                {
                    await sharedRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await associationRepository.AssociationExistsAsync(association.Id)))
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
                    await HandleDbUpdateExceptionOnEdit(ex);
                    return View(associationViewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(associationViewModel);
        }

        // GET: Associations/Delete/5
        /// <summary>
        /// Renders a view of the association delete form.
        /// </summary>
        /// <returns>The rendered view of the association delete form.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var association = await associationRepository.GetAssociationAsync(id.Value);
            if (association is null)
            {
                return NotFound();
            }

            var associationViewModel = associationViewModelMapper.MapAssociationToViewModel(association);
            return View(associationViewModel);
        }

        // POST: Associations/Delete/5
        /// <summary>
        /// Processes the confirmation of intent to delete an association.
        /// </summary>
        /// <param name="id">The Id of the association to delete.</param>
        /// <returns>The rendered <see cref="ActionResult"/> object.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await associationRepository.DeleteAsync(id);
            await sharedRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void AddModelErrorForStringTooLong(DbUpdateException ex)
        {
            string columnName = DbVerificationUtils.GetColumnNameFromDbUpdateException(ex);
            switch (columnName)
            {
                case "'long_name'":
                    DbVerificationUtils.AddModelErrorForStringTooLong(ModelState, "LongName");
                    break;
                case "'short_name'":
                    DbVerificationUtils.AddModelErrorForStringTooLong(ModelState, "ShortName");
                    break;
                default:
                    break;
            }
        }

        private async Task HandleDbUpdateExceptionOnCreate(DbUpdateException ex, Association association)
        {
            var associations = await associationRepository.GetAssociationsAsync();

            if (PrimaryKeyViolationExists(associations, association))
            {
                ModelState.AddModelError("Id", $"{DbVerificationUtils.ErrMsgIntro} An association with the same Id already exists.");
            }
            else
            {
                await HandleDbUpdateExceptionOnEdit(ex, sqlOperation: DbVerificationUtils.SqlOperation.INSERT);
            }
        }

        private async Task HandleDbUpdateExceptionOnEdit(
            DbUpdateException ex, DbVerificationUtils.SqlOperation? sqlOperation = null
        )
        {
            sqlOperation ??= DbVerificationUtils.SqlOperation.UPDATE;

            if (DbVerificationUtils.StringTooLong(ex))
            {
                AddModelErrorForStringTooLong(ex);
            }
            else if (DbVerificationUtils.UniqueKeyConstraintExists(ex.InnerException.Message))
            {
                DbVerificationUtils.AddModelErrorForUniqueKeyConstraintConflict(ModelState, ex.InnerException.Message);
            }
            else if (
                DbVerificationUtils.ForeignKeyConstraintConflictExists(sqlOperation.ToString(), ex.InnerException.Message)
            )
            {
                DbVerificationUtils.AddModelErrorForForeignKeyConstraintConflict(ModelState, ex.InnerException.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"{DbVerificationUtils.ErrMsgIntro} An unexpected error occurred.");
            }
        }

        private static bool PrimaryKeyViolationExists(IEnumerable<Association> associations, Association association)
        {
            return associations.Any(a => a.Id == association.Id);
        }
    }
}
