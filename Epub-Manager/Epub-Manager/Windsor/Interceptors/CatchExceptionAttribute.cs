using System;
using System.Diagnostics;

namespace Epub_Manager.Windsor.Interceptors
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CatchExceptionAttribute : Attribute
    {
        [DebuggerStepThrough]
        public CatchExceptionAttribute(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}