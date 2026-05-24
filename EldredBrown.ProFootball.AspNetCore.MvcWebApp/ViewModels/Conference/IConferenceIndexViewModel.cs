using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public interface IConferenceIndexViewModel
    {
        IEnumerable<ConferenceViewModel> Conferences { get; set; }
    }
}
