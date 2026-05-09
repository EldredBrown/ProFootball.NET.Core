using System;
using System.Collections.ObjectModel;
using System.Linq;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings
{
    public class RankingsControlViewModel : ViewModelBase, IRankingsControlViewModel
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsControlViewModel"/> class.
        /// </summary>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public RankingsControlViewModel(ITeamSeasonRepository teamSeasonRepository)
        {
            _teamSeasonRepository = teamSeasonRepository;
        }

        /// <summary>
        /// Gets or sets the total rankings for this <see cref="RankingsControlViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<TeamSeason>? _totalRankings;
        public ReadOnlyCollection<TeamSeason>? TotalRankings
        {
            get
            {
                return _totalRankings;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(TotalRankings)}");
                }
                else if (value != _totalRankings)
                {
                    _totalRankings = value;
                    OnPropertyChanged("TotalRankings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the offensive rankings for this <see cref="RankingsControlViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<TeamSeason>? _offensiveRankings;
        public ReadOnlyCollection<TeamSeason>? OffensiveRankings
        {
            get
            {
                return _offensiveRankings;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(OffensiveRankings)}");
                }
                else if (value != _offensiveRankings)
                {
                    _offensiveRankings = value;
                    OnPropertyChanged("OffensiveRankings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the defensive rankings for this <see cref="RankingsControlViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<TeamSeason>? _defensiveRankings;
        public ReadOnlyCollection<TeamSeason>? DefensiveRankings
        {
            get
            {
                return _defensiveRankings;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(DefensiveRankings)}");
                }
                else if (value != _defensiveRankings)
                {
                    _defensiveRankings = value;
                    OnPropertyChanged("DefensiveRankings");
                }
            }
        }

        /// <summary>
        /// Loads the rankings views for the selected season.
        /// </summary>
        private DelegateCommand? _viewRankingsCommand;
        public DelegateCommand ViewRankingsCommand
        {
            get
            {
                if (_viewRankingsCommand is null)
                {
                    _viewRankingsCommand = new DelegateCommand(param => ViewRankings());
                }
                return _viewRankingsCommand;
            }
        }
        private void ViewRankings()
        {
            var teamSeasons = _teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason).ToList();
            TotalRankings = new ReadOnlyCollection<TeamSeason>(teamSeasons);
            OffensiveRankings = new ReadOnlyCollection<TeamSeason>(teamSeasons);
            DefensiveRankings = new ReadOnlyCollection<TeamSeason>(teamSeasons);
        }

        /// <summary>
        /// Refreshes this <see cref="RankingsControlViewModel"/> object.
        /// </summary>
        public void Refresh()
        {
            ViewRankings();
        }
    }
}
