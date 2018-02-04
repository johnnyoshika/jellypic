using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Common;
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
            var sender = new WebPushSender();
            foreach (var s in DataContext.Subscriptions)
                await sender.SendAsync(
                    s.Endpoint,
                    s.P256DH,
                    s.Auth,
                    new
                    {
                        title = "Push test!",
                        message = "New notification!",
                        path = "profile"
                    });
        }
    }
}
