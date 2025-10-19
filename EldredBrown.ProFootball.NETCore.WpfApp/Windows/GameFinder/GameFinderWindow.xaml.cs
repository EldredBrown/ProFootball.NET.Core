using System.Windows;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder
{
    /// <summary>
    /// Interaction logic for GameFinderWindow.xaml
    /// </summary>
    public partial class GameFinderWindow : Window, IGameFinderWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameFinderWindow"/> class.
        /// </summary>
        public GameFinderWindow(IGameFinderWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is IGameFinderWindowViewModel viewModel) || !viewModel.OK())
            {
                return;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
