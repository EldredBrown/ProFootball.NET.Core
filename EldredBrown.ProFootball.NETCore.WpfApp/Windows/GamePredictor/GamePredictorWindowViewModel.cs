using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;
using EldredBrown.ProFootball.NETCore.WpfApp.Properties;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor
{
    public class GamePredictorWindowViewModel : ViewModelBase, IGamePredictorWindowViewModel
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly IGamePredictorService _gamePredictorService;
        private readonly IMessageBoxService _messageBoxService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePredictorWindowViewModel"/> class.
        /// </summary>
        /// <param name="seasonRepository">
        /// The <see cref="ISeasonRepository"/> object by which season data will be accessed.
        /// </param>
        /// <param name="teamSeasonRepository">
        /// The <see cref="ITeamSeasonRepository"/> object by which season data will be accessed.
        /// </param>
        /// <param name="gamePredictorService">
        /// The <see cref="IGamePredictorService"/> object used to calculate predicted game scores.
        /// </param>
        /// <param name="messageBoxService">
        /// A <see cref="IMessageBoxService"/> object to show message boxes.
        /// </param>
        public GamePredictorWindowViewModel(
            ISeasonRepository seasonRepository,
            ITeamSeasonRepository teamSeasonRepository,
            IGamePredictorService gamePredictorService,
            IMessageBoxService messageBoxService)
        {
            _seasonRepository = seasonRepository;
            _teamSeasonRepository = teamSeasonRepository;
            _gamePredictorService = gamePredictorService;
            _messageBoxService = messageBoxService;
        }

        /// <summary>
        /// Gets or sets the seasons collection for this window's guest.
        /// </summary>
        private ReadOnlyCollection<int>? _guestSeasons;
        public ReadOnlyCollection<int>? GuestSeasons
        {
            get
            {
                return _guestSeasons;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(GuestSeasons)}");
                }
                else if (value != _guestSeasons)
                {
                    _guestSeasons = value;
                    OnPropertyChanged("GuestSeasons");
                    RequestUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected season for this window's guest.
        /// </summary>
        private int _guestSelectedSeason;
        public int GuestSelectedSeason
        {
            get
            {
                return _guestSelectedSeason;
            }
            set
            {
                if (value != _guestSelectedSeason)
                {
                    _guestSelectedSeason = value;
                    OnPropertyChanged("GuestSelectedSeason");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this window's guest.
        /// </summary>
        private string _guestName = string.Empty;
        public string GuestName
        {
            get
            {
                return _guestName;
            }
            set
            {
                if (value != _guestName)
                {
                    _guestName = value;
                    OnPropertyChanged("GuestName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the predicted score for this window's guest.
        /// </summary>
        private int? _guestScore;
        public int? GuestScore
        {
            get
            {
                return _guestScore;
            }
            set
            {
                if (value != _guestScore)
                {
                    _guestScore = value;
                    OnPropertyChanged("GuestScore");
                }
            }
        }

        /// <summary>
        /// Gets or sets the seasons collection for this window's host.
        /// </summary>
        private ReadOnlyCollection<int>? _hostSeasons;
        public ReadOnlyCollection<int>? HostSeasons
        {
            get
            {
                return _hostSeasons;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(HostSeasons)}");
                }
                else if (value != _hostSeasons)
                {
                    _hostSeasons = value;
                    OnPropertyChanged("HostSeasons");
                    RequestUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected season for this window's host.
        /// </summary>
        private int _hostSelectedSeason;
        public int HostSelectedSeason
        {
            get
            {
                return _hostSelectedSeason;
            }
            set
            {
                if (value != _hostSelectedSeason)
                {
                    _hostSelectedSeason = value;
                    OnPropertyChanged("HostSelectedSeason");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this window's host.
        /// </summary>
        private string _hostName = string.Empty;
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                if (value != _hostName)
                {
                    _hostName = value;
                    OnPropertyChanged("HostName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the predicted score for this window's host.
        /// </summary>
        private int? _hostScore;
        public int? HostScore
        {
            get
            {
                return _hostScore;
            }
            set
            {
                if (value != _hostScore)
                {
                    _hostScore = value;
                    OnPropertyChanged("HostScore");
                }
            }
        }

        /// <summary>
        /// Calculates the predicted score of a future or hypothetical game.
        /// </summary>
        private DelegateCommand? _calculatePredictionCommand;
        public DelegateCommand CalculatePredictionCommand
        {
            get
            {
                if (_calculatePredictionCommand is null)
                {
                    _calculatePredictionCommand = new DelegateCommand(param => CalculatePrediction());
                }
                return _calculatePredictionCommand;
            }
        }
        private void CalculatePrediction()
        {
            var (valid, matchup, message) = ValidateDataEntry();
            if (!valid)
            {
                _messageBoxService.Show(message!, "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var (guestScore, hostScore) = 
                _gamePredictorService.PredictGameScore(matchup!.Value.GuestSeason, matchup!.Value.HostSeason);
            GuestScore = (int?)guestScore;
            HostScore = (int?)hostScore;
        }

        /// <summary>
        /// Loads all the Seasons for this view model.
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
            var years = _seasonRepository.GetSeasons().Select(season => season.Year).ToList();

            GuestSeasons = new ReadOnlyCollection<int>(years);
            GuestSelectedSeason = GuestSeasons.First();

            HostSeasons = new ReadOnlyCollection<int>(years);
            HostSelectedSeason = HostSeasons.First();
        }

        private void MoveFocusTo(string focusedProperty)
        {
            OnMoveFocus(focusedProperty);
        }

        private (bool, Matchup?, string?) ValidateDataEntry()
        {
            if (string.IsNullOrWhiteSpace(GuestName))
            {
                MoveFocusTo("GuestName");
                return (false, null, Settings.Default.BothTeamsNeededErrorMessage);
            }
            else if (string.IsNullOrWhiteSpace(HostName))
            {
                MoveFocusTo("HostName");
                return (false, null, Settings.Default.BothTeamsNeededErrorMessage);
            }

            var guestSeason = _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(GuestName, GuestSelectedSeason);
            var hostSeason = _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(HostName, HostSelectedSeason);

            if (guestSeason is null)
            {
                MoveFocusTo("GuestName");
                return (false, null, Settings.Default.TeamNotInDatabaseErrorMessage);
            }
            else if (hostSeason is null)
            {
                MoveFocusTo("HostName");
                return (false, null, Settings.Default.TeamNotInDatabaseErrorMessage);
            }

            return (true, new Matchup(guestSeason, hostSeason), null);
        }

        private struct Matchup
        {
            public Matchup(TeamSeason guestSeason, TeamSeason hostSeason)
            {
                GuestSeason = guestSeason;
                HostSeason = hostSeason;
            }

            public TeamSeason GuestSeason;
            public TeamSeason HostSeason;
        }
    }
}
