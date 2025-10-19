using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences
{
    public interface IConferencesIndexViewModel
    {
        IEnumerable<Conference> Conferences { get; set; }
    }
}
