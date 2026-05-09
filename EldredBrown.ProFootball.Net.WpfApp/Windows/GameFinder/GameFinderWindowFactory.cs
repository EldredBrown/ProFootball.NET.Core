namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder
{
    public class GameFinderWindowFactory : IGameFinderWindowFactory
    {
        private readonly IGameFinderWindowViewModel _gameFinderWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameFinderWindowFactory"/> class.
        /// </summary>
        /// <param name="gameFinderWindowViewModel">The <see cref="IGameFinderWindowViewModel"/> object to be injected
        /// into the created <see cref="GameFinderWindow"/> object.</param>
        public GameFinderWindowFactory(IGameFinderWindowViewModel gameFinderWindowViewModel)
        {
            _gameFinderWindowViewModel = gameFinderWindowViewModel;
        }

        /// <summary>
        /// Creates an instance of the <see cref="GameFinderWindow"/> class.
        /// </summary>
        /// <returns>An instance of the <see cref="GameFinderWindow"/> class.</returns>
        public IGameFinderWindow CreateWindow()
        {
            return new GameFinderWindow(_gameFinderWindowViewModel);
        }
    }
}
