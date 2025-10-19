using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons
{
    public interface ISeasonsDetailsViewModel
    {
        string Title { get; set; }
        Season Season { get; set; }
    }
}
