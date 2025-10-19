using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Leagues
{
    /// <summary>
    /// Represents the model for a league details view.
    /// </summary>
    public class LeaguesDetailsViewModel : ILeaguesDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the league of the current <see cref="LeaguesDetailsViewModel"/> object.
        /// </summary>
        public League League { get; set; }
    }
}
