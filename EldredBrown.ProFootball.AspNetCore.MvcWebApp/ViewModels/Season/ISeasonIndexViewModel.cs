using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season
{
    public interface ISeasonIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Season> Seasons { get; set; }
    }
}
