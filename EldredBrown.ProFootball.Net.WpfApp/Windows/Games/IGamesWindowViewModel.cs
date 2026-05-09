using System.Collections.ObjectModel;
using System.Windows;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games
{
    public interface IGamesWindowViewModel
    {
        int Week { get; set; }
        string GuestName { get; set; }
        int GuestScore { get; set; }
        string HostName { get; set; }
        int HostScore { get; set; }
        bool IsPlayoff { get; set; }
        bool IsPlayoffEnabled { get; set; }
        string? Notes { get; set; }
        Visibility AddGameControlVisibility { get; set; }
        Visibility EditGameControlVisibility { get; set; }
        Visibility DeleteGameControlVisibility { get; set; }
        ReadOnlyCollection<Game>? Games { get; set; }
        Game? SelectedGame { get; set; }
        bool IsGamesReadOnly { get; set; }
        bool ShowAllGamesEnabled { get; set; }
        bool FindGameFilterApplied { get; set; }
        DelegateCommand AddGameCommand { get; }
        DelegateCommand DeleteGameCommand { get; }
        DelegateCommand EditGameCommand { get; }
        DelegateCommand FindGameCommand { get; }
        DelegateCommand ShowAllGamesCommand { get; }
        DelegateCommand ViewGamesCommand { get; }
    }
}