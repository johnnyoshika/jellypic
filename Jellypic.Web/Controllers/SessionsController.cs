using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Jellypic.Web.Controllers
{
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        public SessionsController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; set; }
        JellypicContext DataContext { get; set; }

        [Authorize]
        [HttpGet("me")]
        public async Task<object> Get() =>
            (await DataContext
                .Users
                .Include(u => u.Posts)
                .FirstAsync(u => u.Id == UserContext.UserId))
                .ToFullJson();

        [HttpPost]
        public async Task Post([FromBody] SessionsPostArgs args)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://graph.facebook.com/me?fields=id,first_name,last_name,name&access_token={args.AccessToken}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var facebookUser = JsonConvert.DeserializeObject<FacebookUserResponse>(data);
            var user = await DataContext.Users.FirstOrDefaultAsync(u => u.AuthType == "Facebook" && u.AuthUserId == facebookUser.id);
            if (user == null)
            {
                user = new Models.User();
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
                DataContext.Users.Add(user);

            await DataContext.SaveChangesAsync();

            // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), ClaimValueTypes.Integer32)
            }, "Cookie");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.Authentication.SignInAsync("JellypicCookie", principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMonths(6),
                IsPersistent = true
            });
        }
    }

    public class SessionsPostArgs
    {
        public string AccessToken { get; set; }
    }

    public class FacebookUserResponse
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
    }

    public class FacebookPictureResponse
    {
        public FacebookPictureDataResponse data { get; set; }
    }

    public class FacebookPictureDataResponse
    {
        public string url { get; set; }
    }
}
