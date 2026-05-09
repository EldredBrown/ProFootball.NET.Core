using System.Collections.ObjectModel;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings
{
    public interface ISeasonStandingsControlViewModel
    {
        ReadOnlyCollection<SeasonTeamStanding>? Standings { get; set; }
        DelegateCommand ViewStandingsCommand { get; }

        void Refresh();
    }
}
