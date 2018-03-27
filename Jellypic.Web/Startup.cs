using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Jellypic.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Jellypic.Web.Base;
using Jellypic.Web.Events;
using Jellypic.Web.Common;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Jellypic.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Env = env;
            Configuration = configuration;
        }

        IHostingEnvironment Env { get; }
        IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigSettings.Current = new ConfigSettings(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.Cookie.Name = "auth-token";
                    o.Events.OnRedirectToLogin = context =>
                     {
                         context.Response.Headers["Location"] = context.RedirectUri;
                         context.Response.StatusCode = 401;
                         return Task.CompletedTask;
                     };
                });

            services.AddMvc(options =>
            {
                if (!Env.IsDevelopment())
                    options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IEventHandler<NotifyEvent>, NotificationWriter>();
            services.AddScoped<IEventHandler<NotifyEvent>, NotificationSender>();
            services.AddScoped<INotificationCreator, NotificationCreator>();
            services.AddScoped<IWebPushSender, WebPushSender>();
            services.AddDbContext<JellypicContext>(options => options.UseSqlServer(Configuration.GetSection("ConnectionStrings")?["DefaultConnection"]), ServiceLifetime.Transient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseMiddleware<NoCacheMiddleware>(new NoCacheOptions("/sw.js"));
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthentication();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<ActivityRecordingMiddleware>();

            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                if (!path.StartsWith("/api") && !path.StartsWith("/privacy") && !Path.HasExtension(path))
                    context.Request.Path = "/";

                await next();
            });
            app.UseMvc();
        }
    }
}
