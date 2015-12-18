using DevExpress.Utils;
using Epub_Manager.Common;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using Epub_Manager.Views.Shell;
using System;
using System.Linq;
using System.Windows;

namespace Epub_Manager.Services
{
    public class LoadingService : ILoadingService
    {
        public IDisposable Show(string message)
        {
            Guard.ArgumentNotNull(message, nameof(message));

            var activeWindow = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(f => f.IsActive);

            var view = activeWindow?.Content as IViewWithLoading;

            if (view == null)
                return new DisposableAction(() => { });

            view.WaitIndicator.Content = message.EnsureIsShortcut();
            view.WaitIndicator.DeferedVisibility = true;

            return new DisposableAction(() =>
            {
                view.WaitIndicator.DeferedVisibility = false;
            });
        }
    }
}