namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor
{
    public class GamePredictorWindowFactory : IGamePredictorWindowFactory
    {
        private readonly IGamePredictorWindowViewModel _gamePredictorWindowViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePredictorWindowFactory"/> class.
        /// </summary>
        /// <param name="gamePredictorWindowViewModel">
        /// The <see cref="IGamePredictorWindowViewModel"/> object to be injected into the created
        /// <see cref="GamePredictorWindow"/> object.</param>
        public GamePredictorWindowFactory(IGamePredictorWindowViewModel gamePredictorWindowViewModel)
        {
            _gamePredictorWindowViewModel = gamePredictorWindowViewModel;
        }

        /// <summary>
        /// Creates an instance of the <see cref="GamePredictorWindow"/> class.
        /// </summary>
        /// <returns>An instance of the <see cref="GamePredictorWindow"/> class.</returns>
        public IGamePredictorWindow CreateWindow()
        {
            return new GamePredictorWindow(_gamePredictorWindowViewModel);
        }
    }
}
