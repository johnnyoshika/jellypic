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
            string sub = securityToken.Claims.First(c => c.Type == "sub").Value;
            string authType = sub.Split('|')[0];
            string authUserId = sub.Split('|')[1];
            using (var dc = DataContext())
            {
                var user = await dc.Users.FirstOrDefaultAsync(u => u.AuthType == authType && u.AuthUserId == authUserId);
                if (user == null)
                {
                    user = new User();
                    user.CreatedAt = DateTime.UtcNow;
                }

                user.Nickname = securityToken.Claims.First(c => c.Type == "nickname").Value;
                user.AuthType = authType;
                user.AuthUserId = authUserId;
                user.FirstName = (securityToken.Claims.FirstOrDefault(c => c.Type == "given_name") ?? securityToken.Claims.FirstOrDefault(c => c.Type == "name"))?.Value;
                user.LastName = securityToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
                user.Gravatar = GravatarUrl(securityToken.Claims.FirstOrDefault(c => c.Type == "picture")?.Value);
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

        string GravatarUrl(string url) => url.Contains("gravatar.com") ? UrlWithoutQuery(url) : null;
        string UrlWithoutQuery(string url) => string.IsNullOrWhiteSpace(url) ? null : new Uri(url).GetLeftPart(UriPartial.Path);
    }
}
