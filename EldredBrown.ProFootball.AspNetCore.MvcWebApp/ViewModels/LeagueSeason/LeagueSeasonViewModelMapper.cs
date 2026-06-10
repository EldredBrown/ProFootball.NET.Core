using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// A class that maps league season data to league season view models.
    /// </summary>
    public class LeagueSeasonViewModelMapper(ILeagueRepository leagueRepository, ISeasonRepository seasonRepository)
        : ILeagueSeasonViewModelMapper
    {
        public LeagueSeasonViewModel MapLeagueSeasonToViewModel(EldredBrown.ProFootball.Net.Data.Models.LeagueSeason LeagueSeason)
        {
            return new LeagueSeasonViewModel { LeagueSeason = LeagueSeason };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.LeagueSeason> MapViewModelToLeagueSeason(
            LeagueSeasonViewModel LeagueSeasonViewModel)
        {
            var LeagueSeason = LeagueSeasonViewModel.LeagueSeason;

            var league = await leagueRepository.GetLeagueByShortNameAsync(LeagueSeasonViewModel.LeagueName);
            LeagueSeason.LeagueId = league is not null ? league.Id : -1;

            var season = await seasonRepository.GetSeasonAsync(LeagueSeasonViewModel.SeasonYear);
            LeagueSeason.SeasonId = season is not null ? season.Id : -1;

            return LeagueSeason;
        }
    }
}
