using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    /// <summary>
    /// Represents the model for a season details view.
    /// </summary>
    public class SeasonsDetailsViewModel : ISeasonsDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the title for the current <see cref="SeasonsDetailsViewModel"/> object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the season of the current <see cref="SeasonsDetailsViewModel"/> object.
        /// </summary>
        public Season Season { get; set; }
    }
}
