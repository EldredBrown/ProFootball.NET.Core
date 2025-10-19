namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder
{
    public interface IGameFinderWindow
    {
        object DataContext { get; set; }

        bool? ShowDialog();
    }
}
