using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    public interface IGameIndexViewModel
    {
        SelectList Seasons { get; set; }
        int? SelectedSeasonYear { get; set; }
        SelectList Weeks { get; set; }
        int? SelectedWeek { get; set; }
        IEnumerable<GameViewModel> Games { get; set; }
    }
}
