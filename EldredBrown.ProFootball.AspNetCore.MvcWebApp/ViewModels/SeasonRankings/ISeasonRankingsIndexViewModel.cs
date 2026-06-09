using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings
{
    public interface ISeasonRankingsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the list that lets users select a season.
        /// </summary>
        SelectList Seasons { get; set; }

        /// <summary>
        /// Gets or sets the year of the selected season for the current view model.
        /// </summary>
        int? SelectedSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the list that lets users select a league.
        /// </summary>
        SelectList Leagues { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected league for the current view model.
        /// </summary>
        string SelectedLeague { get; set; }

        /// <summary>
        /// Gets or sets the list that lets users select a ranking type.
        /// </summary>
        SelectList RankingTypes { get; set; }

        /// <summary>
        /// Gets or sets the year of the selected ranking type for the current view model.
        /// </summary>
        SeasonRankingType SelectedRankingType { get; set; }

        /// <summary>
        /// Gets or sets the collection of season rankings for the current view model.
        /// </summary>
        IEnumerable<IRankingsTeamSeason> SeasonRankings { get; set; }
    }
}
