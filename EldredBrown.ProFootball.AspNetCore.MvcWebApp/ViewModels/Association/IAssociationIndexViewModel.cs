using System.Collections.Generic;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    public interface IAssociationIndexViewModel
    {
        IEnumerable<AssociationViewModel> Associations { get; set; }
    }
}
