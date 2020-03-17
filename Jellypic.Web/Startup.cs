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
using Microsoft.EntityFrameworkCore;
using Jellypic.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Jellypic.Web.Base;
using Jellypic.Web.Events;
using Jellypic.Web.Common;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using GraphQL;
using GraphQL.Validation;
using GraphQL.Server;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Ui.Playground;
using Jellypic.Web.Services;
using Jellypic.Web.GraphQL;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Jellypic.Web.GraphQL.Subscriptions;

namespace Jellypic.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Env = env;
            Configuration = configuration;
        }

        IWebHostEnvironment Env { get; }
        IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigSettings.Current = new ConfigSettings(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => {
                    o.Cookie.Name = "auth-token";
                    o.Cookie.SameSite = SameSiteMode.None; // Allow cookies in CORS when it's cross-domain
                });

            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IUserLogin, UserLogin>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IEventHandler<NotifyEvent>, NotificationWriter>();
            services.AddScoped<IEventHandler<NotifyEvent>, NotificationSender>();
            services.AddScoped<INotificationCreator, NotificationCreator>();
            services.AddScoped<IWebPushSender, WebPushSender>();
            services.AddScoped<IBatchLoader, BatchLoader>();
            services.AddSingleton<PostsAddedSubscriptionService>();
            services.AddSingleton<PostUpdatedSubscriptionService>();
            services.AddDbContext<JellypicContext>(options => options.UseSqlServer(Configuration.GetSection("ConnectionStrings")?["DefaultConnection"]), ServiceLifetime.Transient);
            services.AddTransient<Func<JellypicContext>>(options => () => options.GetService<JellypicContext>());

            services.AddScoped<JellypicSchema>();
            services
                .AddGraphQL(options =>
                {
                    options.ExposeExceptions = Env.IsDevelopment(); // expose detailed exceptions in JSON response
                })
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader()
                .AddWebSockets();
            services
                .AddTransient<IValidationRule, AuthorizationValidationRule>()
                .AddAuthorization(options =>
                {
                    options.AddPolicy("LoggedIn", p => p.RequireAuthenticatedUser());
                });

            services.AddCors();

            // Temporarily allow synchronous IO, as it's required to overcome a bug in GraphQL.Net in .Net Core 3:
            // https://github.com/graphql-dotnet/graphql-dotnet/issues/1161#issuecomment-540197786
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
                builder.WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3000",
                    "http://localhost:8080",
                    "https://localhost:8080",
                    "https://jellypic.koopla.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<ActivityRecordingMiddleware>();
            app.UseWebSockets();
            app.UseGraphQLWebSockets<JellypicSchema>();
            app.UseGraphQL<JellypicSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            app.UseAuthorization();
        }
    }
}
