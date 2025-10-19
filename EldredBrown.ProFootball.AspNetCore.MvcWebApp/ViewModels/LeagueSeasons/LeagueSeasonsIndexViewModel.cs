using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeasons
{
    /// <summary>
    /// Represents the model for a league list view.
    /// </summary>
    public class LeagueSeasonsIndexViewModel : ILeagueSeasonsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of leagues for the current <see cref="LeagueSeasonsIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<LeagueSeason> LeagueSeasons { get; set; }
    }
}
