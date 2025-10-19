using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class ConferencesDetailsViewModel : IConferencesDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the conference of the current <see cref="ConferencesDetailsViewModel"/> object.
        /// </summary>
        public Conference Conference { get; set; }
    }
}
