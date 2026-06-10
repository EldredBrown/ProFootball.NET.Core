using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    public interface ILeagueSeasonIndexViewModel
    {
        SelectList Seasons { get; set; }
        int? SelectedSeasonYear { get; set; }
        IEnumerable<LeagueSeasonViewModel> LeagueSeasons { get; set; }
    }
}
