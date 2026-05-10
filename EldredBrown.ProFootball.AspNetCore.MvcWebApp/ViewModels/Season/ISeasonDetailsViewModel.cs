namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season
{
    public interface ISeasonDetailsViewModel
    {
        string Title { get; set; }
        EldredBrown.ProFootball.Net.Data.Models.Season Season { get; set; }
    }
}
