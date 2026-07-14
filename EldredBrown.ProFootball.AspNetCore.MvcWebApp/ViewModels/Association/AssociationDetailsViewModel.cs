namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    /// <summary>
    /// Represents the model for an association details view.
    /// </summary>
    public class AssociationDetailsViewModel : IAssociationDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the association of the current <see cref="AssociationDetailsViewModel"/> object.
        /// </summary>
        public AssociationViewModel Association { get; set; }
    }
}
