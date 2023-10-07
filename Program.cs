using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Net;
using System.Text;
using TestIdentity.Identity.CustomModel;
using TestIdentity.Identity.Managers;
using TestIdentity.Identity.Stores;

namespace TestIdentity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .Build();
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ITicketStore, TicketStore>();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}