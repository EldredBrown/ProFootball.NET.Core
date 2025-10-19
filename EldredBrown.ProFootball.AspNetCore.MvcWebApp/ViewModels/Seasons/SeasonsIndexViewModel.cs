using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    /// <summary>
    /// Represents the model for a season list view.
    /// </summary>
    public class SeasonsIndexViewModel : ISeasonsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of seasons for the current <see cref="SeasonsIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<Season> Seasons { get; set; }
    }
}
