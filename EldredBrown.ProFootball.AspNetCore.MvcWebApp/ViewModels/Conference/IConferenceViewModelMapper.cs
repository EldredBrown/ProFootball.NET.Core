using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public interface IConferenceViewModelMapper
    {
        ConferenceViewModel MapConferenceToViewModel(EldredBrown.ProFootball.Net.Data.Models.Conference conference);

        Task<EldredBrown.ProFootball.Net.Data.Models.Conference> MapViewModelToConference(
            ConferenceViewModel conferenceViewModel
        );
    }
}
