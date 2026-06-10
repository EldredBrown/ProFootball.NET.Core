using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    public interface ILeagueSeasonViewModelMapper
    {
        LeagueSeasonViewModel MapLeagueSeasonToViewModel(EldredBrown.ProFootball.Net.Data.Models.LeagueSeason leagueSeason);

        Task<EldredBrown.ProFootball.Net.Data.Models.LeagueSeason> MapViewModelToLeagueSeason(
            LeagueSeasonViewModel leagueSeasonViewModel
        );
    }
}
