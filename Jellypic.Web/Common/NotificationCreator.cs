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
        Task CreateAsync(int actorId, int repicientId, int postId, NotificationType type);
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

        public async Task CreateAsync(int actorId, int recipientId, int postId, NotificationType type) =>
            await EventDispatcher.DispatchAsync(new NotifyEvent
            {
                Notification = new Notification
                {
                    ActorId = actorId,
                    Actor = await DataContext.Users.FirstAsync(u => u.Id == actorId),
                    RecipientId = recipientId,
                    Recipient = await DataContext.Users.FirstAsync(u => u.Id == recipientId),
                    PostId = postId,
                    Post = await DataContext.Posts.FirstAsync(p => p.Id == postId),
                    CreatedAt = DateTime.UtcNow,
                    Type = NotificationType.Like
                }
            });
    }
}
