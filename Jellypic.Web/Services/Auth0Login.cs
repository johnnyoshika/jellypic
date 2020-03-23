using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class Auth0Login : IUserLogin
    {
        public Auth0Login(Func<JellypicContext> dataContext, IAuth0TokenReader tokenReader)
        {
            DataContext = dataContext;
            TokenReader = tokenReader;
        }

        Func<JellypicContext> DataContext { get; }
        IAuth0TokenReader TokenReader { get; }

        public async Task<User> LogInAsync(string token) => await UpsertAsync(await TokenReader.ReadAsync(token));

        async Task<User> UpsertAsync(JwtSecurityToken securityToken)
        {
            string auth0UserId = securityToken.Claims.First(c => c.Type == "sub").Value;
            using (var dc = DataContext())
            {
                var user = await dc.Users.FirstOrDefaultAsync(u => u.AuthType == "Auth0" && u.AuthUserId == auth0UserId);
                if (user == null)
                {
                    user = new User();
                    user.CreatedAt = DateTime.UtcNow;
                }

                user.Nickname = securityToken.Claims.First(c => c.Type == "nickname").Value;
                user.AuthType = "Auth0";
                user.AuthUserId = auth0UserId;
                user.FirstName = securityToken.Claims.First(c => c.Type == "name").Value;
                user.Gravatar = UrlWithoutQuery(securityToken.Claims.FirstOrDefault(c => c.Type == "picture")?.Value);
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

        string UrlWithoutQuery(string url) => string.IsNullOrWhiteSpace(url) ? null : new Uri(url).GetLeftPart(UriPartial.Path);
    }
}
