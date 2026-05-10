namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class LeagueSeasonDetailsViewModel : ILeagueSeasonDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the league of the current <see cref="LeagueSeasonDetailsViewModel"/> object.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.LeagueSeason LeagueSeason { get; set; }
    }
}
