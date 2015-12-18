using System;

namespace Epub_Manager.Core.Services
{
    public interface ILoadingService
    {
        IDisposable Show(string message);
    }
}