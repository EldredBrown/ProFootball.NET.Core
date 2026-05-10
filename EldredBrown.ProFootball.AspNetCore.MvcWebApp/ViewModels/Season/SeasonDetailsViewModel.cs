namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season
{
    /// <summary>
    /// Represents the model for a season details view.
    /// </summary>
    public class SeasonDetailsViewModel : ISeasonDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the title for the current <see cref="SeasonDetailsViewModel"/> object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the season of the current <see cref="SeasonDetailsViewModel"/> object.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.Season Season { get; set; }
    }
}
