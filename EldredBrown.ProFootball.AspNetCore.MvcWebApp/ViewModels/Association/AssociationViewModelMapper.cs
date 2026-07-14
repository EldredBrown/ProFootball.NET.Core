using EldredBrown.ProFootball.Net.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    /// <summary>
    /// A class that maps association data to association view models.
    /// </summary>
    public class AssociationViewModelMapper(IAssociationRepository associationRepository)
        : IAssociationViewModelMapper
    {
        public AssociationViewModel MapAssociationToViewModel(
            EldredBrown.ProFootball.Net.Data.Models.Association association
            )
        {
            return new AssociationViewModel { Association = association };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.Association> MapViewModelToAssociation(
            AssociationViewModel associationViewModel)
        {
            var association = associationViewModel.Association;

            var parent = await associationRepository.GetAssociationByShortNameAsync(associationViewModel.ParentName);
            association.ParentId = !associationViewModel.ParentName.IsNullOrEmpty() && parent is null ? -1 : parent?.Id;

            return association;
        }
    }
}
