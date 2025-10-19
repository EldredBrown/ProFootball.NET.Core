using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Leagues
{
    public interface ILeaguesIndexViewModel
    {
        IEnumerable<League> Leagues { get; set; }
    }
}
