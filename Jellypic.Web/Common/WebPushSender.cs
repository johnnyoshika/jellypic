using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebPush;

namespace Jellypic.Web.Common
{
    public class WebPushSender
    {
        public WebPushSender()
        {
            Client = new WebPushClient();

            // get keys from https://web-push-codelab.glitch.me/
            VapidDetails = new VapidDetails(
                "mailto:johnnyoshika@gmail.com",
                ConfigSettings.Current.WebPushVapidKeys.PublicKey,
                ConfigSettings.Current.WebPushVapidKeys.PrivateKey);
        }

        WebPushClient Client { get; }
        VapidDetails VapidDetails { get; }

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
                throw new InvalidOperationException($"Status Code: {ex.StatusCode} \n Endpoint: {ex.PushSubscription.Endpoint}", ex);
            }
        }

    }
}
