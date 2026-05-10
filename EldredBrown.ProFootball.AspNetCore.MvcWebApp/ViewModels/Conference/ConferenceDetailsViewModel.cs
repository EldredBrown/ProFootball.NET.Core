namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    /// <summary>
    /// Represents the model for a conference details view.
    /// </summary>
    public class ConferenceDetailsViewModel : IConferenceDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the conference of the current <see cref="ConferenceDetailsViewModel"/> object.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.Conference Conference { get; set; }
    }
}
