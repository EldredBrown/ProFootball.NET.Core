using System.Windows;
using EldredBrown.ProFootball.NETCore.WpfApp.Properties;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder
{
    public class GameFinderWindowViewModel : ViewModelBase, IGameFinderWindowViewModel
    {
        private readonly IMessageBoxService _messageBoxService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameFinderWindowViewModel"/> class.
        /// </summary>
        /// <param name="messageBoxService">The service that will display message boxes.</param>
        public GameFinderWindowViewModel(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }

        /// <summary>
        /// Gets or sets this <see cref="GameFinderWindowViewModel"/> object's GuestName value.
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
        /// Gets or sets this <see cref="GameFinderWindowViewModel"/> object's HostName value.
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
        /// Handles the WindowLoaded event.
        /// </summary>
        private DelegateCommand? _windowLoadedCommand;
        public DelegateCommand WindowLoadedCommand
        {
            get
            {
                if (_windowLoadedCommand is null)
                {
                    _windowLoadedCommand = new DelegateCommand(param => WindowLoaded());
                }
                return _windowLoadedCommand;
            }
        }
        private void WindowLoaded()
        {
            MoveFocusTo("GuestName");
        }

        public bool OK()
        {
            var (valid, message) = ValidateDataEntry();
            if (!valid)
            {
                _messageBoxService.Show(message!, "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error);
                MoveFocusTo("Guest");
            }

            return valid;
        }

        private void MoveFocusTo(string focusedProperty)
        {
            OnMoveFocus(focusedProperty);
        }

        private (bool, string?) ValidateDataEntry()
        {
            if (string.IsNullOrWhiteSpace(GuestName))
            {
                MoveFocusTo("GuestName");
                return (false, Settings.Default.BothTeamsNeededErrorMessage);
            }
            else if (string.IsNullOrWhiteSpace(HostName))
            {
                MoveFocusTo("HostName");
                return (false, Settings.Default.BothTeamsNeededErrorMessage);
            }
            else if (GuestName == HostName)
            {
                MoveFocusTo("GuestName");
                return (false, Settings.Default.DifferentTeamsNeededErrorMessage);
            }

            return (true, null);
        }
    }
}
