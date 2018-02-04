using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Base;
using Jellypic.Web.Events;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Common
{
    public interface INotificationCreator
    {
        Task CreateAsync(int actorId, Post post, NotificationType type);
    }

    public class NotificationCreator : INotificationCreator
    {
        public NotificationCreator(IEventDispatcher eventDispatcher, JellypicContext dataContext)
        {
            EventDispatcher = eventDispatcher;
            DataContext = dataContext;
        }

        IEventDispatcher EventDispatcher { get; }
        JellypicContext DataContext { get; }

        public async Task CreateAsync(int actorId, Post post, NotificationType type) =>
            await EventDispatcher.DispatchAsync(new NotifyEvent
            {
                Data = new NotifyEventData
                {
                    ActorId = actorId,
                    Post = post,
                    Type = type
                }
            });
    }
}
