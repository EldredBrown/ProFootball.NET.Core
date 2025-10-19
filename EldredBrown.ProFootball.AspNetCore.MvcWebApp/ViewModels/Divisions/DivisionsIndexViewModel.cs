using System.Collections.Generic;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions
{
    /// <summary>
    /// Represents the model for a division list view.
    /// </summary>
    public class DivisionsIndexViewModel : IDivisionsIndexViewModel
    {
        /// <summary>
        /// Gets or sets the collection of divisions for the current <see cref="DivisionsIndexViewModel"/> object.
        /// </summary>
        public IEnumerable<Division> Divisions { get; set; }
    }
}
