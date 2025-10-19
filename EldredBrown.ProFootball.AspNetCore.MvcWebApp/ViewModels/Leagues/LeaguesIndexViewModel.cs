using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Leagues
{
    /// <summary>
    /// Represents the model for a league list view.
    /// </summary>
    public class LeaguesIndexViewModel : ILeaguesIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of leagues for the current <see cref="LeaguesIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<League> Leagues { get; set; }
    }
}
