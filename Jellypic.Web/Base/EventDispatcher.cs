using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jellypic.Web.Events;

namespace Jellypic.Web.Base
{
    public class EventDispatcher : IEventDispatcher
    {
        public EventDispatcher(IServiceProvider provider)
        {
            Provider = provider;
        }

        IServiceProvider Provider { get; set; }

        public Task DispatchAsync(DomainEvent e)
        {
            if (e == null)
                return Task.CompletedTask;

            var handlerType = e.GetType().GetHandlerType().GetEnumerableType();

            var matches = from handler in (IEnumerable<object>)Provider.GetService(handlerType)
                          let method = handler.GetType().GetMethod("HandleAsync", new[] { e.GetType() })
                          select method.InvokeAsync(handler, e);

            return Task.WhenAll(matches);
        }
    }

    static class TypeHelpers
    {
        public static Type GetHandlerType(this Type type) =>
            typeof(IEventHandler<>).MakeGenericType(type);

        public static Type GetEnumerableType(this Type type) =>
            typeof(IEnumerable<>).MakeGenericType(type);
    }

    static class HandlerInvoker
    {
        [DebuggerHidden]
        public static Task InvokeAsync(this MethodInfo method, object handler, object e)
        {
            try
            {
                return (Task)method.Invoke(handler, new[] { e });
            }
            catch (TargetInvocationException ex)
            {
                return Task.FromException(ex.InnerException);
            }
        }
    }
}
