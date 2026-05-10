using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public interface IConferenceIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Conference> Conferences { get; set; }
    }
}
