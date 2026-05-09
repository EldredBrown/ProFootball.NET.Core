using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeasons
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class LeagueSeasonsDetailsViewModel : ILeagueSeasonsDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the league of the current <see cref="LeagueSeasonsDetailsViewModel"/> object.
        /// </summary>
        public LeagueSeason LeagueSeason { get; set; }
    }
}
