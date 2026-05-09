using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions
{
    public interface IDivisionsIndexViewModel
    {
        IEnumerable<Division> Divisions { get; set; }
    }
}
