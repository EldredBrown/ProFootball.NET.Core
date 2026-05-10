namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team
{
    /// <summary>
    /// Represents the model for a team details view.
    /// </summary>
    public class TeamDetailsViewModel : ITeamDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the team of the current <see cref="TeamDetailsViewModel"/> object.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.Team Team { get; set; }
    }
}
