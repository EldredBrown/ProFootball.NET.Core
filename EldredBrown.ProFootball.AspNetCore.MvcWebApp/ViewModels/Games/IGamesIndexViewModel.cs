using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Games
{
    public interface IGamesIndexViewModel
    {
        IEnumerable<Game> Games { get; set; }
        SelectList Seasons { get; set; }
        int SelectedSeasonYear { get; set; }
        int? SelectedWeek { get; set; }
        SelectList Weeks { get; set; }
    }
}
