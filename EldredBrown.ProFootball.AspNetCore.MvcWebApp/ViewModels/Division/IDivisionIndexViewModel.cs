using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    public interface IDivisionIndexViewModel
    {
        IEnumerable<DivisionViewModel> Divisions { get; set; }
    }
}
