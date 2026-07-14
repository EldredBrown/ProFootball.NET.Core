using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    public interface IAssociationViewModelMapper
    {
        AssociationViewModel MapAssociationToViewModel(EldredBrown.ProFootball.Net.Data.Models.Association association);

        Task<EldredBrown.ProFootball.Net.Data.Models.Association> MapViewModelToAssociation(
            AssociationViewModel associationViewModel
        );
    }
}
