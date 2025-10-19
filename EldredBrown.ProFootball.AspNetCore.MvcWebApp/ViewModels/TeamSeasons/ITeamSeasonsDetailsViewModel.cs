using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons
{
    public interface ITeamSeasonsDetailsViewModel
    {
        TeamSeason TeamSeason { get; set; }
        IEnumerable<TeamSeasonOpponentProfile> TeamSeasonScheduleProfile { get; set; }
        TeamSeasonScheduleTotals TeamSeasonScheduleTotals { get; set; }
        TeamSeasonScheduleAverages TeamSeasonScheduleAverages { get; set; }
    }
}
