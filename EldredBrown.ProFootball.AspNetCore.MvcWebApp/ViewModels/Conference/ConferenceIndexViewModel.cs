using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    /// <summary>
    /// Represents the model for a conference list view.
    /// </summary>
    public class ConferenceIndexViewModel : IConferenceIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of conferences for the current <see cref="ConferenceIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Conference> Conferences { get; set; }
    }
}
