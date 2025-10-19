using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games
{
    /// <summary>
    /// Interaction logic for GamesWindow.xaml
    /// </summary>
    public partial class GamesWindow : Window, IGamesWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamesWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="IGamesWindowViewModel"/> that will serve as the data context for
        /// instances of this class.</param>
        public GamesWindow(IGamesWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void GamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesDataGrid.SelectedItem == CollectionView.NewItemPlaceholder)
            {
                if (!(DataContext is IGamesWindowViewModel viewModel))
                {
                    return;
                }

                viewModel.SelectedGame = null;
            }
        }
    }
}
