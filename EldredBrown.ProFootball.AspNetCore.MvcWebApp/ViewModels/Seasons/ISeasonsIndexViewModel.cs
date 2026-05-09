using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    public interface ISeasonsIndexViewModel
    {
        IEnumerable<Season> Seasons { get; set; }
    }
}
