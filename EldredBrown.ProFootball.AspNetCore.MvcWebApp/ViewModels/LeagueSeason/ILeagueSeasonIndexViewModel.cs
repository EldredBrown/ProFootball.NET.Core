using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    public interface ILeagueSeasonIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.LeagueSeason> LeagueSeasons { get; set; }
    }
}
