using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    public interface IGameIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Game> Games { get; set; }
        SelectList Seasons { get; set; }
        int SelectedSeasonYear { get; set; }
        SelectList Weeks { get; set; }
        int? SelectedWeek { get; set; }
    }
}
