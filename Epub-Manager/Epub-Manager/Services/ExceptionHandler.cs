using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using System;
using System.Windows;

namespace Epub_Manager.Services
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IMessageManager _messageManager;

        public ExceptionHandler(IMessageManager messageManager)
        {
            Guard.ArgumentNotNull(messageManager, nameof(messageManager));
            this._messageManager = messageManager;
        }

        public void Handle(Exception exception)
        {
            Guard.ArgumentNotNull(exception, nameof(exception));

            var prefix = exception is EpubException
                ? string.Empty
                : $"An Unknown Error has occured.{Environment.NewLine}";

            this._messageManager.Show(prefix + exception.GetFullMessage(), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}