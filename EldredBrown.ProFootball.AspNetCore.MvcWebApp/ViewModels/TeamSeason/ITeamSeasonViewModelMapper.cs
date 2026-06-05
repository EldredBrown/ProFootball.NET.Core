using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public interface ITeamSeasonViewModelMapper
    {
        TeamSeasonViewModel MapTeamSeasonToViewModel(EldredBrown.ProFootball.Net.Data.Models.TeamSeason teamSeason);

        Task<EldredBrown.ProFootball.Net.Data.Models.TeamSeason> MapViewModelToTeamSeason(
            TeamSeasonViewModel teamSeasonViewModel
        );
    }
}
