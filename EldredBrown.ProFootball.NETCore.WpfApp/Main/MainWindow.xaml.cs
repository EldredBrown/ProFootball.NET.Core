using System.Windows;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The <see cref="IMainWindowViewModel"/> object that will serve as this <see cref="MainWindow"/> object's
        /// data context.
        /// </param>
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();

            viewModel.TeamSeasonsControlViewModel = 
                TeamSeasonsControl.DataContext as ITeamSeasonsControlViewModel;
            viewModel.SeasonStandingsControlViewModel =
                SeasonStandingsControl.DataContext as ISeasonStandingsControlViewModel;
            viewModel.RankingsControlViewModel =
                RankingsControl.DataContext as IRankingsControlViewModel;

            DataContext = viewModel;
        }
    }
}
