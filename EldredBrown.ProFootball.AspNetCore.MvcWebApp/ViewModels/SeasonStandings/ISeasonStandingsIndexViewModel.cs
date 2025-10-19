using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings
{
    public interface ISeasonStandingsIndexViewModel
    {
        SelectList Seasons { get; set; }
        int SelectedSeasonYear { get; set; }
        IEnumerable<SeasonTeamStanding> SeasonStandings { get; set; }
    }
}