using System.Collections.ObjectModel;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons
{
    public interface ITeamSeasonsControlViewModel
    {
        ReadOnlyCollection<TeamSeason>? Teams { get; set; }
        TeamSeason? SelectedTeam { get; set; }
        ReadOnlyCollection<TeamSeasonOpponentProfile>? TeamSeasonScheduleProfile { get; }
        ReadOnlyCollection<TeamSeasonScheduleTotals>? TeamSeasonScheduleTotals { get; }
        ReadOnlyCollection<TeamSeasonScheduleAverages>? TeamSeasonScheduleAverages { get; }
        DelegateCommand ViewTeamScheduleCommand { get; }
        DelegateCommand ViewTeamsCommand { get; }

        void Refresh();
    }
}
