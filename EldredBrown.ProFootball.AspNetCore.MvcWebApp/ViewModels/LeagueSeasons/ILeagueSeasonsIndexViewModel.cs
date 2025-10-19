using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeasons
{
    public interface ILeagueSeasonsIndexViewModel
    {
        IEnumerable<LeagueSeason> LeagueSeasons { get; set; }
    }
}
