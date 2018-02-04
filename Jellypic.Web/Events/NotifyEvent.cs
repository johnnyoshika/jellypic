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
        public Notification Notification { get; set; }
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
            DataContext.Notifications.Add(e.Notification);
            await DataContext.SaveChangesAsync();
        }
    }

    public class NotificationSender : IEventHandler<NotifyEvent>
    {
        public NotificationSender(JellypicContext dataContext)
        {
            DataContext = dataContext;
        }

        JellypicContext DataContext { get; }

        public async Task HandleAsync(NotifyEvent e)
        {
            var pushSender = new WebPushSender();
            foreach (var subscription in await DataContext.Subscriptions.Where(s => s.UserId == e.Notification.RecipientId).ToListAsync())
                await pushSender.SendAsync(subscription.Endpoint, subscription.P256DH, subscription.Auth, new
                {
                    title = $"New {e.Notification.Type}",
                    message = $"You received a new {e.Notification.Type}",
                    path = $"posts/{e.Notification.PostId}"
                });
        }
    }
}
