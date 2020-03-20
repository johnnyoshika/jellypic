using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class FacebookLogin : IUserLogin
    {
        public FacebookLogin(Func<JellypicContext> dataContext, ISignIn signIn)
        {
            DataContext = dataContext;
            SignIn = signIn;
        }

        Func<JellypicContext> DataContext { get; }
        public ISignIn SignIn { get; }

        public async Task<User> LogInAsync(string token)
        {
            var user = await UpsertAsync(await FacebookUserAsync(token));
            await SignIn.SignInAsync(user.Id.ToString());
            return user;
        }

        async Task<FacebookUserResponse> FacebookUserAsync(string facebookAccessToken)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://graph.facebook.com/me?fields=id,first_name,last_name,name&access_token={facebookAccessToken}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserResponse>(data);
        }

        async Task<User> UpsertAsync(FacebookUserResponse facebookUser)
        {
            using (var dc = DataContext())
            {
                var user = await dc.Users.FirstOrDefaultAsync(u => u.AuthType == "Facebook" && u.AuthUserId == facebookUser.id);
                if (user == null)
                {
                    user = new User();
                    user.CreatedAt = DateTime.UtcNow;
                }

                user.Username = Regex.Replace(facebookUser.name.ToLower(), "[^a-z0-9]", string.Empty);
                user.AuthType = "Facebook";
                user.AuthUserId = facebookUser.id;
                user.FirstName = facebookUser.first_name;
                user.LastName = facebookUser.last_name;
                user.LastActivityAt = DateTime.UtcNow;
                user.LastLoggedInAt = DateTime.UtcNow;
                user.ActivityCount++;
                user.LoginCount++;

                if (user.Id == 0)
                    dc.Users.Add(user);

                await dc.SaveChangesAsync();
                return user;
            }
        }

        class FacebookUserResponse
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string name { get; set; }
        }

        class FacebookPictureResponse
        {
            public FacebookPictureDataResponse data { get; set; }
        }

        class FacebookPictureDataResponse
        {
            public string url { get; set; }
        }
    }
}
