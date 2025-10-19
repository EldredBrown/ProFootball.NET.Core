using System.Collections.ObjectModel;
using System.Linq;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings
{
    public class SeasonStandingsControlViewModel : ViewModelBase, ISeasonStandingsControlViewModel
    {
        private readonly ISeasonStandingsRepository _seasonStandingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonStandingsControlViewModel"/> class.
        /// </summary>
        /// <param name="seasonStandingsRepository">
        /// The repository by which season standings data will be accessed.
        /// </param>
        public SeasonStandingsControlViewModel(ISeasonStandingsRepository seasonStandingsRepository)
        {
            _seasonStandingsRepository = seasonStandingsRepository;
        }

        /// <summary>
        /// Gets or sets the standings for this <see cref="SeasonStandingsControlViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<SeasonTeamStanding>? _standings;
        public ReadOnlyCollection<SeasonTeamStanding>? Standings
        {
            get
            {
                return _standings;
            }
            set
            {
                if (value != _standings)
                {
                    _standings = value;
                    OnPropertyChanged("Standings");
                }
            }
        }

        /// <summary>
        /// Loads the standings view for the selected season.
        /// </summary>
        private DelegateCommand? _viewStandingsCommand;
        public DelegateCommand ViewStandingsCommand
        {
            get
            {
                if (_viewStandingsCommand is null)
                {
                    _viewStandingsCommand = new DelegateCommand(param => ViewStandings());
                }
                return _viewStandingsCommand;
            }
        }
        private void ViewStandings()
        {
            var seasonStandings = _seasonStandingsRepository.GetSeasonStandings(WpfGlobals.SelectedSeason);
            Standings = new ReadOnlyCollection<SeasonTeamStanding>(seasonStandings.ToList());
        }

        /// <summary>
        /// Refreshes this <see cref="SeasonStandingsControlViewModel"/> object.
        /// </summary>
        public void Refresh()
        {
            ViewStandings();
        }
    }
}
