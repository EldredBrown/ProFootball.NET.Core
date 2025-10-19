using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams
{
    /// <summary>
    /// Represents the model for a team details view.
    /// </summary>
    public class TeamsDetailsViewModel : ITeamsDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the team of the current <see cref="TeamsDetailsViewModel"/> object.
        /// </summary>
        public Team Team { get; set; }
    }
}
