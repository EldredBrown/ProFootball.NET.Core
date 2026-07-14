using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    /// <summary>
    /// Represents the model for an association list view.
    /// </summary>
    public class AssociationIndexViewModel : IAssociationIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of associations for the current <see cref="AssociationIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<AssociationViewModel> Associations { get; set; }
    }
}
