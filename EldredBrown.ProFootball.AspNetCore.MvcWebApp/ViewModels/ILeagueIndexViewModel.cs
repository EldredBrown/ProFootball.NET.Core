using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    public interface ILeagueIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.League> Leagues { get; set; }
    }
}
