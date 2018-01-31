using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebPush;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    public class NotificationController : Controller
    {
        public NotificationController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpGet]
        public async Task<object> Get() =>
            await DataContext
                .Notifications
                .Include(n => n.Post)
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == UserContext.UserId && !n.Dismissed)
                .Select(n => n.ToJson())
                .ToListAsync();

        [HttpPost("send")]
        public async Task Send()
        {
            // get keys from https://web-push-codelab.glitch.me/
            var details = new VapidDetails(
                "mailto:johnnyoshika@gmail.com",
                ConfigSettings.Current.WebPushVapidKeys.PublicKey,
                ConfigSettings.Current.WebPushVapidKeys.PrivateKey);

            var clients = new WebPushClient();
            foreach (var subscription in await DataContext.Subscriptions.Select(s => new PushSubscription(s.Endpoint, s.P256DH, s.Auth)).ToListAsync())
                await clients.SendNotificationAsync(subscription, JsonConvert.SerializeObject(new
                {
                    message = "hello"
                }), details);
        }
    }
}
