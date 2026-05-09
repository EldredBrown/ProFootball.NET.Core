using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams
{
    public interface ITeamsIndexViewModel
    {
        IEnumerable<Team> Teams { get; set; }
    }
}
