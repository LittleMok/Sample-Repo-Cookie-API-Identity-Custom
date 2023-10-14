using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Net;
using TestIdentity.DataAccess;
using TestIdentity.Identity.CustomModel;
using TestIdentity.Identity.Managers;
using TestIdentity.Identity.Stores;

namespace TestIdentity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication
                .CreateBuilder(args);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            builder.Configuration
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables($"{typeof(Program).Namespace}_");

            builder.Services.AddControllers();

            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConn");
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            builder.Services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, $"Data-Protection-Keys-{typeof(Program).Namespace}");

            builder.Services.AddSingleton<ITicketStore, TicketStore>();
            builder.Services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddConsole();
            });
            builder.Services
                   .AddIdentity<AppUser, AppRole>()
                   .AddUserManager<AppUserManager>()
                   .AddUserStore<UserStore>()
                   .AddRoleStore<RoleStore>();

            builder.Services
                .AddAuthentication();

            builder.Services.ConfigureApplicationCookie((configure) =>
            {
                configure.Cookie.Name = "TestIdentityCookie";
                configure.Events.OnRedirectToLogin = c =>
                {
                    c.Response.Clear();
                    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    c.RedirectUri = string.Empty;
                    return Task.CompletedTask;
                };
                configure.Events.OnRedirectToAccessDenied = c =>
                {
                    c.Response.Clear();
                    c.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    c.RedirectUri = string.Empty;
                    return Task.CompletedTask;
                };
            });
            builder.Services
                .AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
                .Configure<ITicketStore>((options, store) => options.SessionStore = store);

            builder.Services.AddAuthorization();
            builder.Services.AddDatabase(builder.Configuration, "Default");
            builder.Services.AddScoped<IRoleStore<AppRole>, RoleStore>();
            builder.Services.AddScoped<IUserStore<AppUser>, UserStore>();

            var app = builder.Build();

            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();

                var context = app.Services.GetRequiredService<DataAccess.AppContext>();
                context.Database.EnsureCreated();
            }
            // Configure the HTTP request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Use((ctx, next) =>
            {
                var hostName = Dns.GetHostName();
                var myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                ctx.Response.Headers.Add("X-Machine-Ip", myIP);
                ctx.Response.Headers.Add("X-Machine-Name", hostName);

                return next.Invoke();
            });
            app.Run();
        }
    }
}