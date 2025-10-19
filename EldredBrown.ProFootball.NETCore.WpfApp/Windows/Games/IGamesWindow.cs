namespace EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games
{
    public interface IGamesWindow
    {
        object DataContext { get; set; }

        bool? ShowDialog();
    }
}
