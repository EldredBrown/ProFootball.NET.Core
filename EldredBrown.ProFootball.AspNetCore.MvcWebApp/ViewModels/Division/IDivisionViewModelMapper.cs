using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    public interface IDivisionViewModelMapper
    {
        DivisionViewModel MapDivisionToViewModel(EldredBrown.ProFootball.Net.Data.Models.Division division);

        Task<EldredBrown.ProFootball.Net.Data.Models.Division> MapViewModelToDivision(
            DivisionViewModel divisionViewModel
        );
    }
}
