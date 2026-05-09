using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences
{
    public interface IConferencesIndexViewModel
    {
        IEnumerable<Conference> Conferences { get; set; }
    }
}
