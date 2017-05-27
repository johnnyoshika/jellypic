using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Jellypic.Web.Controllers
{
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        public SessionsController(JellypicContext context)
        {
            Context = context;
        }

        public JellypicContext Context { get; set; }

        [HttpGet("me")]
        public IActionResult Get()
        {
            return Content("Hello world!");
        }

        [HttpPost]
        public async Task Post([FromBody] SessionsPostArgs args)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://graph.facebook.com/me?fields=id,first_name,last_name,name&access_token={args.AccessToken}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var facebookUser = JsonConvert.DeserializeObject<FacebookUserResponse>(data);
            var user = Context.Users.FirstOrDefault(u => u.AuthType == "Facebook" && u.AuthUserId == facebookUser.Id);
            if (user == null)
            {
                user = new Models.User();
                user.CreatedAt = DateTime.UtcNow;
            }

            user.Username = Regex.Replace(facebookUser.Name.ToLower(), "[^a-z0-9]", string.Empty);
            user.AuthType = "Facebook";
            user.AuthUserId = facebookUser.Id;
            user.FirstName = facebookUser.FirstName;
            user.LastName = facebookUser.LastName;
            user.LastActivityAt = DateTime.UtcNow;
            user.LastLoggedInAt = DateTime.UtcNow;
            user.ActivityCount++;
            user.LoginCount++;

            var pictureResponse = await client.GetAsync($"https://graph.facebook.com/me/picture?redirect=false&height=320&width=320&return_ssl_resources=true&access_token={args.AccessToken}");
            var pictureData = await pictureResponse.Content.ReadAsStringAsync();
            var facebookPicture = JsonConvert.DeserializeObject<FacebookPictureResponse>(pictureData);
            user.PictureUrl = facebookPicture.Data.Url;

            if (user.Id == 0)
                Context.Users.Add(user);

            await Context.SaveChangesAsync();

            // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", user.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim("Username", user.Username, ClaimValueTypes.String)
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
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
    }

    public class FacebookPictureResponse
    {
        public FacebookPictureDataResponse Data { get; set; }
    }

    public class FacebookPictureDataResponse
    {
        public string Url { get; set; }
    }
}
