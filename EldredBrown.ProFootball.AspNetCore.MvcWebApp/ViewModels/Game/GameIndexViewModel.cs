using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    /// <summary>
    /// Represents the model for a game list view.
    /// </summary>
    public class GameIndexViewModel : IGameIndexViewModel
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
        public IEnumerable<GameViewModel> Games { get; set; }
    }
}
