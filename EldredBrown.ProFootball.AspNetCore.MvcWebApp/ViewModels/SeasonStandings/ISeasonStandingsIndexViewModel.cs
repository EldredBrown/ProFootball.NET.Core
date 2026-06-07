using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings
{
    public interface ISeasonStandingsIndexViewModel
    {
        SelectList Seasons { get; set; }
        int? SelectedSeasonYear { get; set; }
        IEnumerable<SeasonTeamStanding> SeasonStandings { get; set; }
    }
}
