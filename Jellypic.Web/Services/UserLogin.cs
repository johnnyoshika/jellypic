using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class UserLogin : IUserLogin
    {
        public UserLogin(Func<JellypicContext> dataContext, IHttpContextAccessor contextAccessor)
        {
            DataContext = dataContext;
            HttpContext = contextAccessor.HttpContext;
        }

        Func<JellypicContext> DataContext { get; }
        HttpContext HttpContext { get; }

        public async Task<User> LogInAsync(string token)
        {
            var user = await UpsertAsync(await FacebookUserAsync(token));

            await SignInAsync(user);

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

        async Task SignInAsync(User user)
        {
            // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), ClaimValueTypes.Integer32)
            }, "Cookie");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMonths(6),
                IsPersistent = true
            });
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
