using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    /// <summary>
    /// A class that maps league season data to league season view models.
    /// </summary>
    public class LeagueSeasonViewModelMapper(IAssociationRepository leagueRepository, ISeasonRepository seasonRepository)
        : ILeagueSeasonViewModelMapper
    {
        internal readonly IAssociationRepository _leagueRepository = leagueRepository;
        internal readonly ISeasonRepository _seasonRepository = seasonRepository;

        public LeagueSeasonViewModel MapLeagueSeasonToViewModel(EldredBrown.ProFootball.Net.Data.Models.LeagueSeason LeagueSeason)
        {
            return new LeagueSeasonViewModel { LeagueSeason = LeagueSeason };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.LeagueSeason> MapViewModelToLeagueSeason(
            LeagueSeasonViewModel LeagueSeasonViewModel)
        {
            var LeagueSeason = LeagueSeasonViewModel.LeagueSeason;

            var league = await _leagueRepository.GetAssociationByShortNameAsync(LeagueSeasonViewModel.LeagueName);
            LeagueSeason.LeagueId = league is null ? -1 : league.Id;

            return LeagueSeason;
        }
    }
}
