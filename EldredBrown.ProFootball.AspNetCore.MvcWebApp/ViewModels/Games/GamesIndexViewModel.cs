using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Games
{
    /// <summary>
    /// Represents the model for a game list view.
    /// </summary>
    public class GamesIndexViewModel : IGamesIndexViewModel
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
        /// Gets or sets the list that lets users select a week.
        /// </summary>
        public SelectList Weeks { get; set; }

        /// <summary>
        /// Gets or sets the selected week for the current view model.
        /// </summary>
        public int? SelectedWeek { get; set; }

        /// <summary>
        /// Gets or sets the collection of games for the current view model.
        /// </summary>
        public IEnumerable<Game> Games { get; set; }
    }
}
