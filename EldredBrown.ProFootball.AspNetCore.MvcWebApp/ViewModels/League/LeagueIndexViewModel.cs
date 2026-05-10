using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    /// <summary>
    /// Represents the model for a league list view.
    /// </summary>
    public class LeagueIndexViewModel : ILeagueIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of leagues for the current <see cref="LeagueIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<EldredBrown.ProFootball.Net.Data.Models.League> Leagues { get; set; }
    }
}
