using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams
{
    public interface ITeamsIndexViewModel
    {
        IEnumerable<Team> Teams { get; set; }
    }
}
