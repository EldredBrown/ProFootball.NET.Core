using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team
{
    /// <summary>
    /// Represents the model for a team list view.
    /// </summary>
    public class TeamIndexViewModel : ITeamIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of teams for the current <see cref="TeamIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Team> Teams { get; set; }
    }
}
