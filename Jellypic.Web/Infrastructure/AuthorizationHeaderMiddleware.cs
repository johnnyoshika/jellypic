using Jellypic.Web.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Jellypic.Web.Infrastructure
{
    public class AuthorizationHeaderMiddleware
    {
        public AuthorizationHeaderMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        RequestDelegate Next { get; }

        public async Task Invoke(HttpContext httpContext, IEncryptor encryptor)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var value))
            {
                if (encryptor.TryDecrypt(value, out string result))
                {
                    var identity = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim("UserId", result, ClaimValueTypes.Integer32)
                    }, "AuthorizationHeader");

                    httpContext.User = new ClaimsPrincipal(identity);
                }
                
            }

            await Next(httpContext);
        }
    }
}
