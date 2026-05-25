namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class LeagueDetailsViewModel : ILeagueDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the league of the current <see cref="LeagueDetailsViewModel"/> object.
        /// </summary>
        public LeagueViewModel League { get; set; }
    }
}
