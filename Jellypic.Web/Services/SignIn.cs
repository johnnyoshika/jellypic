using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class SignIn : ISignIn
    {
        public SignIn(IHttpContextAccessor contextAccessor)
        {
            HttpContext = contextAccessor.HttpContext;
        }

        HttpContext HttpContext { get; }

        public async Task SignInAsync(string userId)
        {
            // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", userId, ClaimValueTypes.Integer32)
            }, "Cookie");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMonths(6),
                IsPersistent = true
            });
        }
    }
}
