using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public interface ITeamSeasonIndexViewModel
    {
        SelectList Seasons { get; set; }
        int SelectedSeasonYear { get; set; }
        IEnumerable<EldredBrown.ProFootball.Net.Data.Decorators.TeamSeasonDecorator> TeamSeasons { get; set; }
    }
}
