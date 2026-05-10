using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team
{
    public interface ITeamIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Team> Teams { get; set; }
    }
}
