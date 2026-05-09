using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder
{
    public interface IGameFinderWindowViewModel
    {
        string GuestName { get; set; }
        string HostName { get; set; }
        DelegateCommand WindowLoadedCommand { get; }

        bool OK();
    }
}
