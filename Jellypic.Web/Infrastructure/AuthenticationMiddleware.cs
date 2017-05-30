using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Jellypic.Web.Infrastructure
{
    public class AuthenticationMiddleware
    {
        public AuthenticationMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        RequestDelegate Next { get; }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
                httpContext.Items.Add(
                    UserContext.UserItem,
                    int.Parse(
                        ((ClaimsIdentity)httpContext.User.Identity).ValueFromType("UserId")));

            await Next(httpContext);
        }
    }

    static class IdentityExtensions
    {
        public static string ValueFromType(this ClaimsIdentity identity, string type) =>
            identity.Claims.FirstOrDefault(c => c.Type == type)?.Value;
    }
}
