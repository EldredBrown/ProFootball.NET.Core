using System.Collections.ObjectModel;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor
{
    public interface IGamePredictorWindowViewModel
    {
        ReadOnlyCollection<int>? GuestSeasons { get; set; }
        int GuestSelectedSeason { get; set; }
        string GuestName { get; set; }
        int? GuestScore { get; set; }

        ReadOnlyCollection<int>? HostSeasons { get; set; }
        int HostSelectedSeason { get; set; }
        string HostName { get; set; }
        int? HostScore { get; set; }

        DelegateCommand CalculatePredictionCommand { get; }
        DelegateCommand ViewSeasonsCommand { get; }
    }
}
