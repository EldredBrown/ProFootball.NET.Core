using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    public interface ISeasonsDetailsViewModel
    {
        string Title { get; set; }
        Season Season { get; set; }
    }
}
