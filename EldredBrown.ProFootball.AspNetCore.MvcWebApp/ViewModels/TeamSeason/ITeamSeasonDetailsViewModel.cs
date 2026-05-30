using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public interface ITeamSeasonDetailsViewModel
    {
        EldredBrown.ProFootball.Net.Data.Models.TeamSeason TeamSeason { get; set; }
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.TeamSeasonOpponentProfile> TeamSeasonScheduleProfile { get; set; }
        EldredBrown.ProFootball.Net.Data.Models.TeamSeasonScheduleTotals TeamSeasonScheduleTotals { get; set; }
        EldredBrown.ProFootball.Net.Data.Models.TeamSeasonScheduleAverages TeamSeasonScheduleAverages { get; set; }
    }
}
