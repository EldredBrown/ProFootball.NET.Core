using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    public interface IDivisionIndexViewModel
    {
        IEnumerable<EldredBrown.ProFootball.Net.Data.Models.Division> Divisions { get; set; }
    }
}
