using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    /// <summary>
    /// A class that maps league data to league view models.
    /// </summary>
    public class LeagueViewModelMapper : ILeagueViewModelMapper
    {
        private readonly ISeasonRepository _seasonRepository;

        public LeagueViewModelMapper(ISeasonRepository seasonRepository)
        {
            _seasonRepository = seasonRepository;
        }

        public LeagueViewModel MapLeagueToViewModel(
            EldredBrown.ProFootball.Net.Data.Models.League league
        )
        {
            return new LeagueViewModel { League = league };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.League> MapViewModelToLeague(
            LeagueViewModel leagueViewModel
        )
        {
            var league = leagueViewModel.League;

            var firstSeason = await _seasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear);
            league.FirstSeasonId = firstSeason.Id;

            if (leagueViewModel.LastSeasonYear is not null)
            {
                var lastSeason = await _seasonRepository.GetSeasonAsync(leagueViewModel.LastSeasonYear.Value);
                league.LastSeasonId = lastSeason.Id;
            }

            return league;
        }
    }
}
