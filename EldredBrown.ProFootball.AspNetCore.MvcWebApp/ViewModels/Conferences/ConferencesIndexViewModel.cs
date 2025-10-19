using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences
{
    /// <summary>
    /// Represents the model for a conference list view.
    /// </summary>
    public class ConferencesIndexViewModel : IConferencesIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of conferences for the current <see cref="ConferencesIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<Conference> Conferences { get; set; }
    }
}
