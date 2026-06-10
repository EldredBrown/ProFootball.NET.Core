namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// Represents the model for a team season details view.
    /// </summary>
    public class LeagueSeasonDetailsViewModel : ILeagueSeasonDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the team season of the current view model.
        /// </summary>
        public LeagueSeasonViewModel LeagueSeason { get; set; }
    }
}
