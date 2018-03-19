using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jellypic.Web.Infrastructure
{
    public class NoCacheMiddleware
    {
        public NoCacheMiddleware(RequestDelegate next, NoCacheOptions options)
        {
            Next = next;
            Options = options;
        }

        RequestDelegate Next { get; }
        NoCacheOptions Options { get; }

        public async Task Invoke(HttpContext httpContext)
        {
            await Next(httpContext);

            // If response 'has started', then it's most likely that an exception was thrown (e.g. NotFoundException for 404 response).
            // if response has started, then trying to change the response will throw an exception, but regardless, we shouldn't be setting cache control headers in this situation anyways.
            if (!httpContext.Response.HasStarted && Options.Paths.Contains(httpContext.Request.Path.Value))
            {
                // https://stackoverflow.com/a/2068407/188740
                httpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                httpContext.Response.Headers.Add("Pragma", "no-cache");
                httpContext.Response.Headers.Add("Expires", "0");
            }
        }
    }

    public class NoCacheOptions
    {
        public NoCacheOptions(params string[] paths)
        {
            Paths = paths;
        }

        public string[] Paths { get; }
    }
}
