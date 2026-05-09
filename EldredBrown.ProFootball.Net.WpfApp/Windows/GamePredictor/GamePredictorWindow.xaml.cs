using System.Windows;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor
{
    /// <summary>
    /// Interaction logic for GameFinderWindow.xaml
    /// </summary>
    public partial class GamePredictorWindow : Window, IGamePredictorWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePredictorWindow"/> class.
        /// </summary>
        public GamePredictorWindow(IGamePredictorWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
