using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    /// <summary>
    /// A class that maps conference data to conference view models.
    /// </summary>
    public class ConferenceViewModelMapper(ILeagueRepository leagueRepository, ISeasonRepository seasonRepository) : IConferenceViewModelMapper
    {
        public ConferenceViewModel MapConferenceToViewModel(
            EldredBrown.ProFootball.Net.Data.Models.Conference conference
        )
        {
            return new ConferenceViewModel { Conference = conference };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.Conference> MapViewModelToConference(
            ConferenceViewModel conferenceViewModel
        )
        {
            var conference = conferenceViewModel.Conference;

            var parentLeague = await leagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName);
            conference.LeagueId = parentLeague?.ShortName is not null ? parentLeague.Id : -1;

            var firstSeason = await seasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear);
            conference.FirstSeasonId = firstSeason is not null ? firstSeason.Id : -1;

            if (conferenceViewModel.LastSeasonYear is not null)
            {
                var lastSeason = await seasonRepository.GetSeasonAsync(conferenceViewModel.LastSeasonYear.Value);
                conference.LastSeasonId = lastSeason is not null ? lastSeason.Id : -1;
            }

            return conference;
        }
    }
}
