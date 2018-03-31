using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/subscriptions")]
    public class SubscriptionController : Controller
    {
        public SubscriptionController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpGet]
        public async Task<object> Get(string endpoint)
        {
            var subscription = await DataContext.Subscriptions.FirstOrDefaultAsync(s => s.Endpoint == endpoint);
            if (subscription == null)
                throw new NotFoundException("Subscription not found.");

            if (subscription.UserId != UserContext.UserId)
                throw new UnauthorizedAccessException();

            return subscription.ToJson();
        }

        [HttpPost]
        public async Task Post([FromBody] SubscriptionPostArgs args)
        {
            DataContext.Subscriptions.Add(new Subscription
            {
                UserId = UserContext.UserId,
                Endpoint = args.Endpoint,
                P256DH = args.Keys.P256DH,
                Auth = args.Keys.Auth,
                CreatedAt = DateTime.UtcNow
            });

            await DataContext.SaveChangesAsync();
        }

        [HttpDelete]
        public async Task Delete(string endpoint)
        {
            var subscription = await DataContext.Subscriptions.FirstOrDefaultAsync(s => s.Endpoint == endpoint);
            if (subscription == null)
                return;

            DataContext.Subscriptions.Remove(subscription);
            await DataContext.SaveChangesAsync();
        }
    }

    public class SubscriptionPostArgs
    {
        public string Endpoint { get; set; }
        public SubscriptionPostKeysArgs Keys { get; set; }
    }

    public class SubscriptionPostKeysArgs
    {
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}
