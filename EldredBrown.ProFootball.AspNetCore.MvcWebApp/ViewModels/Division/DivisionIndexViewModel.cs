using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    /// <summary>
    /// Represents the model for a division list view.
    /// </summary>
    public class DivisionIndexViewModel : IDivisionIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of divisions for the current <see cref="DivisionIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<DivisionViewModel> Divisions { get; set; }
    }
}
