namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games
{
    public class GamesWindowFactory : IGamesWindowFactory
    {
        private readonly IGamesWindowViewModel _gamesWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesWindowFactory"/> class.
        /// </summary>
        /// <param name="gamesWindowViewModel">The <see cref="IGamesWindowViewModel"/> object to be injected into
        /// the created <see cref="GamesWindow"/> object.</param>
        public GamesWindowFactory(IGamesWindowViewModel gamesWindowViewModel)
        {
            _gamesWindowViewModel = gamesWindowViewModel;
        }

        /// <summary>
        /// Creates an instance of the <see cref="GamesWindow"/> class.
        /// </summary>
        /// <returns>An instance of the <see cref="GamesWindow"/> class.</returns>
        public IGamesWindow CreateWindow()
        {
            return new GamesWindow(_gamesWindowViewModel);
        }
    }
}
