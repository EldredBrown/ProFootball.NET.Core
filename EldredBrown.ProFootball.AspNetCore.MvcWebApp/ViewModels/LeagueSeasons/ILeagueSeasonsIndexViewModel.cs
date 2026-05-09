using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeasons
{
    public interface ILeagueSeasonsIndexViewModel
    {
        IEnumerable<LeagueSeason> LeagueSeasons { get; set; }
    }
}
