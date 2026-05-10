using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// Represents the model for a league list view.
    /// </summary>
    public class LeagueSeasonIndexViewModel : ILeagueSeasonIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of leagues for the current <see cref="LeagueSeasonIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<EldredBrown.ProFootball.Net.Data.Models.LeagueSeason> LeagueSeasons { get; set; }
    }
}
