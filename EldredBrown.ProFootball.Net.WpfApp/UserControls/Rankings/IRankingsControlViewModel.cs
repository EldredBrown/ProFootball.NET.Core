using System.Collections.ObjectModel;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings
{
    public interface IRankingsControlViewModel
    {
        ReadOnlyCollection<TeamSeason>? TotalRankings { get; set; }
        ReadOnlyCollection<TeamSeason>? OffensiveRankings { get; set; }
        ReadOnlyCollection<TeamSeason>? DefensiveRankings { get; set; }
        DelegateCommand ViewRankingsCommand { get; }

        void Refresh();
    }
}
