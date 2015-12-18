using System.Windows;

namespace Epub_Manager.Core.Services
{
    public interface IMessageManager
    {
        MessageBoxResult Show(string message, string title, MessageBoxButton buttons, MessageBoxImage image);
    }
}