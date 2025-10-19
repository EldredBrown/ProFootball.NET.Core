using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    public interface ISeasonsIndexViewModel
    {
        IEnumerable<Season> Seasons { get; set; }
    }
}
