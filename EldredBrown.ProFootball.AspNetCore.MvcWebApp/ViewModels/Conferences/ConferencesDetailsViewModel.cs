using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences
{
    /// <summary>
    /// Represents the model for a conference details view.
    /// </summary>
    public class ConferencesDetailsViewModel : IConferencesDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the conference of the current <see cref="ConferencesDetailsViewModel"/> object.
        /// </summary>
        public Conference Conference { get; set; }
    }
}
