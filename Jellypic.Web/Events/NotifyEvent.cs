using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Base;
using Jellypic.Web.Common;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Events
{
    public class NotifyEvent : DomainEvent
    {
        public NotifyEventData Data { get; set; }
    }

    public class NotifyEventData
    {
        public int ActorId { get; set; }
        public Post Post { get; set; }
        public NotificationType Type { get; set; }
    }


    public class NotificationWriter : IEventHandler<NotifyEvent>
    {
        public NotificationWriter(JellypicContext dataContext)
        {
            DataContext = dataContext;
        }

        JellypicContext DataContext { get; }

        public async Task HandleAsync(NotifyEvent e)
        {
            DataContext.Notifications.Add(new Notification
            {
                ActorId = e.Data.ActorId,
                RecipientId = e.Data.Post.UserId,
                PostId = e.Data.Post.Id,
                CreatedAt = DateTime.UtcNow,
                Type = e.Data.Type
            });
            await DataContext.SaveChangesAsync();
        }
    }

    public class NotificationSender : IEventHandler<NotifyEvent>
    {
        public NotificationSender(JellypicContext dataContext, IWebPushSender webPushSender)
        {
            DataContext = dataContext;
            WebPushSender = webPushSender;
        }

        JellypicContext DataContext { get; }
        IWebPushSender WebPushSender { get; }

        public async Task HandleAsync(NotifyEvent e)
        {
            var cloudinary = new CloudinaryUrlBuilder();
            foreach (var subscription in await DataContext.Subscriptions.Where(s => s.UserId == e.Data.Post.UserId).ToListAsync())
            {
                var actor = await DataContext.Users.FirstAsync(u => u.Id == e.Data.ActorId);
                await WebPushSender.SendAsync(subscription.Endpoint, subscription.P256DH, subscription.Auth, new
                {
                    title = $"New {e.Data.Type}",
                    message = $"From {actor.FirstName} {actor.LastName} (@{actor.Username})",
                    postId = e.Data.Post.Id,
                    icon = cloudinary.Square(e.Data.Post.CloudinaryPublicId, 196), // At least 192px recommended: https://developers.google.com/web/fundamentals/push-notifications/display-a-notification
                    //badge = "" // TODO
                });
            }
        }
    }
}
