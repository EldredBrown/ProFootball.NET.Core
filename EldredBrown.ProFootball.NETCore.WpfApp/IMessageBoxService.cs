using System.Windows;

namespace EldredBrown.ProFootball.NETCore.WpfApp
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}