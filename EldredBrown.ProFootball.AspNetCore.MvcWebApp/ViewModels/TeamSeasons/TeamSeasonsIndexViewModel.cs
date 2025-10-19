using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons
{
    /// <summary>
    /// Represents the model for a team season list view.
    /// </summary>
    public class TeamSeasonsIndexViewModel : ITeamSeasonsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the list that lets users select a season.
        /// </summary>
        public SelectList Seasons { get; set; }

        /// <summary>
        /// Gets or sets the year of the selected season for the current view model.
        /// </summary>
        public int SelectedSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the collection of team seasons for the current view model.
        /// </summary>
        public IEnumerable<TeamSeason> TeamSeasons { get; set; }
    }
}
