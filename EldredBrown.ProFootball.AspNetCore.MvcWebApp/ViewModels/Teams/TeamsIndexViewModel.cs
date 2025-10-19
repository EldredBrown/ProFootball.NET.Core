using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams
{
    /// <summary>
    /// Represents the model for a team list view.
    /// </summary>
    public class TeamsIndexViewModel : ITeamsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of teams for the current <see cref="TeamsIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<Team> Teams { get; set; }
    }
}
