using DevExpress.Utils;
using DevExpress.Xpf.Core;
using Epub_Manager.Core.Services;
using System.Windows;

namespace Epub_Manager.Services
{
    public class MessageManager : IMessageManager
    {
        public MessageBoxResult Show(string message, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            Guard.ArgumentNotNull(message, nameof(message));
            Guard.ArgumentNotNull(title, nameof(title));

            return DXMessageBox.Show(message, title, buttons, image);
        }
    }
}