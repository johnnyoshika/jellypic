using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebPush;

namespace Jellypic.Web.Common
{
    public interface IWebPushSender
    {
        Task SendAsync(string endpoint, string p256dh, string auth, object payload);
    }

    public class WebPushSender : IWebPushSender
    {
        public WebPushSender(JellypicContext dataContext)
        {
            DataContext = dataContext;

            Client = new WebPushClient();

            // get keys from https://web-push-codelab.glitch.me/
            VapidDetails = new VapidDetails(
                "mailto:johnnyoshika@gmail.com",
                ConfigSettings.Current.WebPushVapidKeys.PublicKey,
                ConfigSettings.Current.WebPushVapidKeys.PrivateKey);
        }

        WebPushClient Client { get; }
        VapidDetails VapidDetails { get; }
        JellypicContext DataContext { get; }

        public async Task SendAsync(string endpoint, string p256dh, string auth, object payload)
        {
            try
            {
                await Client.SendNotificationAsync(
                    new PushSubscription(endpoint, p256dh, auth),
                    JsonConvert.SerializeObject(payload),
                    VapidDetails);
            }
            catch (WebPushException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Gone)
                {
                    DataContext.Subscriptions.Remove(
                        await DataContext.Subscriptions.FirstAsync(s => s.Endpoint == endpoint));
                    await DataContext.SaveChangesAsync();
                    return;
                }

                throw new InvalidOperationException($"Status Code: {ex.StatusCode} \n Endpoint: {ex.PushSubscription.Endpoint}", ex);
            }
        }

    }
}
