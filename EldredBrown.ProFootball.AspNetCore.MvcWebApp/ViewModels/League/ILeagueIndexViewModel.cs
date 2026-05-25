using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    public interface ILeagueIndexViewModel
    {
        IEnumerable<LeagueViewModel> Leagues { get; set; }
    }
}
