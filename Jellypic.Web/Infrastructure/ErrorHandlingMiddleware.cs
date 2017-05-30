using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Jellypic.Web.Infrastructure
{
    public class ErrorHandlingMiddleware
    {
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        RequestDelegate Next { get; }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);

                if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Not authorized!");

                if (httpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
                    throw new NotFoundException($"'{httpContext.Request.Path.Value}' not found.");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is NotFoundException)
                code = HttpStatusCode.NotFound;

            if (exception is UnauthorizedAccessException)
                code = HttpStatusCode.Unauthorized;

            if (exception is BadRequestException)
                code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { message = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
