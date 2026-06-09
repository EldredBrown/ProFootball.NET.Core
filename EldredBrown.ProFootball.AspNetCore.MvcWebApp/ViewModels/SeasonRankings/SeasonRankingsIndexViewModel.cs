using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings
{
    /// <summary>
    /// Represents the model for a season rankings view.
    /// </summary>
    public class SeasonRankingsIndexViewModel : ISeasonRankingsIndexViewModel
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
        /// Gets or sets the list that lets users select a league.
        /// </summary>
        public SelectList Leagues { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected league for the current view model.
        /// </summary>
        public string SelectedLeague { get; set; }

        /// <summary>
        /// Gets or sets the list that lets users select a ranking type.
        /// </summary>
        public SelectList RankingTypes { get; set; }

        /// <summary>
        /// Gets or sets the year of the selected ranking type for the current view model.
        /// </summary>
        public SeasonRankingType SelectedRankingType { get; set; }

        /// <summary>
        /// Gets or sets the collection of season rankings for the current view model.
        /// </summary>
        public IEnumerable<IRankingsTeamSeason> SeasonRankings { get; set; }
    }
}
