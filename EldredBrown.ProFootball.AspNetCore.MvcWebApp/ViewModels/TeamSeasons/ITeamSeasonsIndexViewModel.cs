using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons
{
    public interface ITeamSeasonsIndexViewModel
    {
        SelectList Seasons { get; set; }
        int SelectedSeasonYear { get; set; }
        IEnumerable<TeamSeason> TeamSeasons { get; set; }
    }
}
