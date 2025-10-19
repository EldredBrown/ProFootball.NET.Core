using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions
{
    public interface IDivisionsIndexViewModel
    {
        IEnumerable<Division> Divisions { get; set; }
    }
}
