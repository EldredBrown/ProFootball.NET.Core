using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// Represents the model for a team season list view.
    /// </summary>
    public class LeagueSeasonIndexViewModel : ILeagueSeasonIndexViewModel
    {
        /// <summary>
        /// Gets or sets the list that lets users select a season.
        /// </summary>
        public SelectList Seasons { get; set; }

        /// <summary>
        /// Gets or sets the year of the selected season for the current view model.
        /// </summary>
        public int? SelectedSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the collection of team seasons for the current view model.
        /// </summary>
        public IEnumerable<LeagueSeasonViewModel> LeagueSeasons { get; set; }
    }
}
