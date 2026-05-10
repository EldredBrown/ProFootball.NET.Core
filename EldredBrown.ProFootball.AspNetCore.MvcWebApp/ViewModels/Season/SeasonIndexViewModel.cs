using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season
{
    /// <summary>
    /// Represents the model for a season list view.
    /// </summary>
    public class SeasonIndexViewModel : ISeasonIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of seasons for the current <see cref="SeasonIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Season> Seasons { get; set; }
    }
}
