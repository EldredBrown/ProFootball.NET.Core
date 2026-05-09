using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class DivisionsDetailsViewModel : IDivisionsDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the division of the current <see cref="DivisionsDetailsViewModel"/> object.
        /// </summary>
        public Division Division { get; set; }
    }
}
