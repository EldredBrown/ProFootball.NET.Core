using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Leagues
{
    public interface ILeaguesIndexViewModel
    {
        IEnumerable<League> Leagues { get; set; }
    }
}
