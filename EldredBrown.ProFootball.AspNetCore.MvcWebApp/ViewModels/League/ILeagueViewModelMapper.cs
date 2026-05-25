using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    public interface ILeagueViewModelMapper
    {
        LeagueViewModel MapLeagueToViewModel(EldredBrown.ProFootball.Net.Data.Models.League league);

        Task<EldredBrown.ProFootball.Net.Data.Models.League> MapViewModelToLeague(
            LeagueViewModel leagueViewModel
        );
    }
}
