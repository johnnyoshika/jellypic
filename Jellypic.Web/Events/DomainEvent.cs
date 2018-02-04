using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellypic.Web.Events
{
    public abstract class DomainEvent
    {
        ConcurrentHashSet<Func<DomainEvent, Task>> Handlers { get; } =
            new ConcurrentHashSet<Func<DomainEvent, Task>>();

        public async Task ReplyAsync(DomainEvent reply) =>
            await Task.WhenAll(from h in Handlers
                               select h(reply));

        public DomainEvent OnReply<T>(Action<T> handler) 
            where T : DomainEvent =>
            OnReply<T>(async reply => handler(reply));

        public DomainEvent OnReply<T>(Func<T, Task> handler)
            where T : DomainEvent
        {
            Handlers.Add(async reply =>
            {
                if (reply is T)
                    await handler(reply as T);
            });

            return this;
        }               
    }
}
