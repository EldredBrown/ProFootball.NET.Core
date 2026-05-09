using System.Collections.ObjectModel;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Main
{
    public interface IMainWindowViewModel
    {
        ReadOnlyCollection<int>? Seasons { get; set; }
        int SelectedSeason { get; set; }
        ITeamSeasonsControlViewModel? TeamSeasonsControlViewModel { get; set; }
        ISeasonStandingsControlViewModel? SeasonStandingsControlViewModel { get; set; }
        IRankingsControlViewModel? RankingsControlViewModel { get; set; }
        DelegateCommand PredictGameScoreCommand { get; }
        DelegateCommand ViewSeasonsCommand { get; }
        DelegateCommand WeeklyUpdateCommand { get; }
    }
}
