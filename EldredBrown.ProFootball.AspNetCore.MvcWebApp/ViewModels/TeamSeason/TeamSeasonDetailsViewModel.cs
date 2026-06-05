using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    /// <summary>
    /// Represents the model for a team season details view.
    /// </summary>
    public class TeamSeasonDetailsViewModel : ITeamSeasonDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the team season of the current view model.
        /// </summary>
        public TeamSeasonViewModel TeamSeason { get; set; }

        /// <summary>
        /// Gets or sets the team season schedule profile of the current view model.
        /// </summary>
        public IEnumerable<TeamSeasonOpponentProfile> TeamSeasonScheduleProfile { get; set; }

        /// <summary>
        /// Gets or sets the team season schedule totals of the current view model.
        /// </summary>
        public TeamSeasonScheduleTotals TeamSeasonScheduleTotals { get; set; }

        /// <summary>
        /// Gets or sets the team season schedule averages of the current view model.
        /// </summary>
        public TeamSeasonScheduleAverages TeamSeasonScheduleAverages { get; set; }
    }
}
