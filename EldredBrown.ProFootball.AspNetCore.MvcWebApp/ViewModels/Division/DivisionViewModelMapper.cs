using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    /// <summary>
    /// A class that maps division data to division view models.
    /// </summary>
    public class DivisionViewModelMapper : IDivisionViewModelMapper
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IConferenceRepository _conferenceRepository;
        private readonly ISeasonRepository _seasonRepository;

        public DivisionViewModelMapper(
            ILeagueRepository leagueRepository,
            IConferenceRepository conferenceRepository,
            ISeasonRepository seasonRepository
            )
        {
            _leagueRepository = leagueRepository;
            _conferenceRepository = conferenceRepository;
            _seasonRepository = seasonRepository;
        }

        public DivisionViewModel MapDivisionToViewModel(
            EldredBrown.ProFootball.Net.Data.Models.Division division
        )
        {
            return new DivisionViewModel { Division = division };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.Division> MapViewModelToDivision(
            DivisionViewModel divisionViewModel
        )
        {
            var division = divisionViewModel.Division;

            var parentLeague = await _leagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName);
            division.LeagueId = parentLeague?.ShortName is not null ? parentLeague.Id : -1;

            if (divisionViewModel.ConferenceName is not null)
            {
                var parentConference = await _conferenceRepository.GetConferenceByShortNameAsync(
                    divisionViewModel.ConferenceName);
                division.ConferenceId = parentConference?.ShortName is not null ? parentConference.Id : -1;
            }

            var firstSeason = await _seasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear);
            division.FirstSeasonId = firstSeason is not null ? firstSeason.Id : -1;

            if (divisionViewModel.LastSeasonYear is not null)
            {
                var lastSeason = await _seasonRepository.GetSeasonAsync(divisionViewModel.LastSeasonYear.Value);
                division.LastSeasonId = lastSeason is not null ? lastSeason.Id : -1;
            }

            return division;
        }
    }
}
