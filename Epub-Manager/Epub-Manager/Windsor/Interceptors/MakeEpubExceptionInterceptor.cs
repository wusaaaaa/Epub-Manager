using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Epub_Manager.Core;

namespace Epub_Manager.Windsor.Interceptors
{
    [DebuggerStepThrough]
    public class MakeEpubExceptionInterceptor : IInterceptor
    {
        [DebuggerStepThrough]
        public void Intercept(IInvocation invocation)
        {
            if (this.HasAttribute(invocation) == false)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                invocation.Proceed();

                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    MethodInfo replaceTaskMethod = invocation.Method.ReturnType.GenericTypeArguments.Any()
                        ? this
                            .GetType()
                            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                            .First(f => f.Name == nameof(WrapTask) && f.IsGenericMethod)
                            .MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments)
                        : this
                            .GetType()
                            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                            .First(f => f.Name == nameof(WrapTask) && f.IsGenericMethod == false);

                    invocation.ReturnValue = replaceTaskMethod.Invoke(this, new[] { invocation, invocation.ReturnValue });
                }
            }
            catch (Exception exception) when (exception is EpubException == false)
            {
                this.HandleException(invocation, exception);
            }
        }

        [DebuggerStepThrough]
        private Task<T> WrapTask<T>(IInvocation invocation, Task<T> task)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await task;
                    return task.Result;
                }
                catch (Exception exception) when (exception is EpubException == false)
                {
                    this.HandleException(invocation, exception);
                    return default(T);
                }
            });
        }

        [DebuggerStepThrough]
        private Task WrapTask(IInvocation invocation, Task task)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await task;
                }
                catch (Exception exception) when (exception is EpubException == false)
                {
                    this.HandleException(invocation, exception);
                }
            });
        }

        [DebuggerStepThrough]
        private void HandleException(IInvocation invocation, Exception exception)
        {
            throw new EpubException(this.GetMessage(invocation), exception);
        }

        [DebuggerStepThrough]
        private bool HasAttribute(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget.GetCustomAttributes(typeof(CatchExceptionAttribute), true).Any();
        }

        [DebuggerStepThrough]
        private string GetMessage(IInvocation invocation)
        {
            return invocation.MethodInvocationTarget
                .GetCustomAttributes(typeof(CatchExceptionAttribute), true)
                .Cast<CatchExceptionAttribute>()
                .First()
                .Message;
        }
    }
}