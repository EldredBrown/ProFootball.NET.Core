using System.Windows.Controls;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons
{
    /// <summary>
    /// Interaction logic for TeamSeasonsControl.xaml
    /// </summary>
    public partial class TeamSeasonsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonsControl"/> class.
        /// </summary>
        public TeamSeasonsControl()
        {
            InitializeComponent();

            if (App.ServiceProvider is null)
            {
                return;
            }

            DataContext = App.ServiceProvider.GetService(typeof(ITeamSeasonsControlViewModel));
        }
    }
}
