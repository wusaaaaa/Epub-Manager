using DevExpress.Utils;
using System;

namespace Epub_Manager.Common
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            Guard.ArgumentNotNull(action, nameof(action));

            this._action = action;
        }

        public void Dispose()
        {
            this._action();
        }
    }
}