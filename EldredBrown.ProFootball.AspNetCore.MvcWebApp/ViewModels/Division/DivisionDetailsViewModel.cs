namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    /// <summary>
    /// Represents the model for a division details view.
    /// </summary>
    public class DivisionDetailsViewModel : IDivisionDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the division of the current <see cref="DivisionDetailsViewModel"/> object.
        /// </summary>
        public DivisionViewModel Division { get; set; }
    }
}
