using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public interface ITeamSeasonIndexViewModel
    {
        SelectList Seasons { get; set; }
        int? SelectedSeasonYear { get; set; }
        IEnumerable<TeamSeasonViewModel> TeamSeasons { get; set; }
    }
}
