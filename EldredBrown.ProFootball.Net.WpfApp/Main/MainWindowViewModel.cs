using System;
using System.Collections.ObjectModel;
using System.Linq;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Main
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly IGamesWindowFactory _gamesWindowFactory;
        private readonly IGamePredictorWindowFactory _gamePredictorWindowFactory;
        private readonly IWeeklyUpdateService _weeklyUpdateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> object by which season data will be accessed.
        /// </param>
        /// <param name="gamesWindowFactory">
        /// The <see cref="IGamesWindowFactory"/> object by which the attached games window will be created.
        /// </param>
        /// <param name="gamePredictorWindowFactory">
        /// The <see cref="IGamePredictorWindowFactory"/> object by which the attached game predictor window will be
        /// created.
        /// </param>
        /// <param name="weeklyUpdateService">
        /// The <see cref="IWeeklyUpdateService"/> object that will run the weekly update.
        /// </param>
        public MainWindowViewModel(
            ISeasonRepository seasonRepository,
            IGamesWindowFactory gamesWindowFactory,
            IGamePredictorWindowFactory gamePredictorWindowFactory,
            IWeeklyUpdateService weeklyUpdateService)
        {
            _seasonRepository = seasonRepository;
            _gamesWindowFactory = gamesWindowFactory;
            _gamePredictorWindowFactory = gamePredictorWindowFactory;
            _weeklyUpdateService = weeklyUpdateService;
        }

        /// <summary>
        /// Gets or sets the seasons collection for this <see cref="MainWindowViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<int>? _seasons;
        public ReadOnlyCollection<int>? Seasons
        {
            get
            {
                return _seasons;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(Seasons)}");
                }
                else if (value != _seasons)
                {
                    _seasons = value;
                    OnPropertyChanged("Seasons");
                    RequestUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected season for this <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public int SelectedSeason
        {
            get
            {
                return WpfGlobals.SelectedSeason;
            }
            set
            {
                if (value != WpfGlobals.SelectedSeason)
                {
                    WpfGlobals.SelectedSeason = value;
                    OnPropertyChanged("SelectedSeason");

                    RefreshControls();
                }
            }
        }

        /// <summary>
        /// Gets or sets the TeamSeasonsControlViewModel for this <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public ITeamSeasonsControlViewModel? TeamSeasonsControlViewModel { get; set; }

        /// <summary>
        /// Gets or sets the SeasonStandingsControlViewModel for this <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public ISeasonStandingsControlViewModel? SeasonStandingsControlViewModel { get; set; }

        /// <summary>
        /// Gets or sets the RankingsControlViewModel for this <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public IRankingsControlViewModel? RankingsControlViewModel { get; set; }

        /// <summary>
        /// Opens the game predictor window.
        /// </summary>
        private DelegateCommand? _predictGameScoreCommand;
        public DelegateCommand PredictGameScoreCommand
        {
            get
            {
                if (_predictGameScoreCommand is null)
                {
                    _predictGameScoreCommand = new DelegateCommand(param => PredictGameScore());
                }
                return _predictGameScoreCommand;
            }
        }
        private void PredictGameScore()
        {
            var gamePredictorWindow = _gamePredictorWindowFactory.CreateWindow();
            gamePredictorWindow.Show();
        }

        /// <summary>
        /// Runs a weekly update.
        /// </summary>
        private DelegateCommand? _weeklyUpdateCommand;
        public DelegateCommand WeeklyUpdateCommand
        {
            get
            {
                if (_weeklyUpdateCommand is null)
                {
                    _weeklyUpdateCommand = new DelegateCommand(param => RunWeeklyUpdate());
                }
                return _weeklyUpdateCommand;
            }
        }
        private void RunWeeklyUpdate()
        {
            _weeklyUpdateService.RunWeeklyUpdate(WpfGlobals.SelectedSeason);
        }

        /// <summary>
        /// Shows the Games window.
        /// </summary>
        private DelegateCommand? _showGamesCommand;
        public DelegateCommand ShowGamesCommand
        {
            get
            {
                if (_showGamesCommand is null)
                {
                    _showGamesCommand = new DelegateCommand(param => ShowGames());
                }
                return _showGamesCommand;
            }
        }
        private void ShowGames()
        {
            var gamesWindow = _gamesWindowFactory.CreateWindow();
            gamesWindow.ShowDialog();

            if (TeamSeasonsControlViewModel is null)
            {
                return;
            }

            TeamSeasonsControlViewModel.Refresh();
        }

        /// <summary>
        /// Loads data into the Seasons control.
        /// </summary>
        private DelegateCommand? _viewSeasonsCommand;
        public DelegateCommand ViewSeasonsCommand
        {
            get
            {
                if (_viewSeasonsCommand is null)
                {
                    _viewSeasonsCommand = new DelegateCommand(param => ViewSeasons());
                }
                return _viewSeasonsCommand;
            }
        }
        private void ViewSeasons()
        {
            var seasons = _seasonRepository.GetSeasons().Select(s => s.Year);
            Seasons = new ReadOnlyCollection<int>(seasons.ToList());
            SelectedSeason = Seasons.First();
        }

        private void RefreshControls()
        {
            if (TeamSeasonsControlViewModel is null ||
                SeasonStandingsControlViewModel is null ||
                RankingsControlViewModel is null)
            {
                return;
            }

            TeamSeasonsControlViewModel.Refresh();
            SeasonStandingsControlViewModel.Refresh();
            RankingsControlViewModel.Refresh();
        }
    }
}
