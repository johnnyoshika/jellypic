using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Infrastructure
{
    public class ActivityRecordingMiddleware
    {
        public ActivityRecordingMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        RequestDelegate Next { get; }

        public async Task Invoke(HttpContext httpContext, IUserContext userContext, JellypicContext dataContext)
        {
            // GraphQL Playground continuously sends introspection queries that inflate the activity count.
            // Those requests contain the header 'X-Apollo-Tracing'.
            if (!httpContext.Request.Headers.ContainsKey("X-Apollo-Tracing") && httpContext.User.Identity.IsAuthenticated)
            {
                var user = await dataContext.Users.FirstOrDefaultAsync(u => u.Id == userContext.UserId);
                if (user != null)
                {
                    user.LastActivityAt = DateTime.UtcNow;
                    user.ActivityCount++;

                    await dataContext.SaveChangesAsync();
                }
            }

            await Next(httpContext);
        }
    }
}
