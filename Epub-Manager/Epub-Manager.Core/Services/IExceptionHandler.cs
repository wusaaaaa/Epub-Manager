using System;

namespace Epub_Manager.Core.Services
{
    public interface IExceptionHandler
    {
        void Handle(Exception exception);
    }
}