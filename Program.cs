using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestIdentity.Identity;
using TestIdentity.Identity.CustomModel;
using TestIdentity.Identity.Managers;
using TestIdentity.Identity.Stores;
using TestIdentity.JwtAuthorization;

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
            builder.Services.Configure<JwtConfiguration>(configuration.GetRequiredSection(nameof(JwtConfiguration)));

            builder.Services
                .AddIdentity<AppUser, AppRole>()
                .AddUserManager<AppUserManager>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configure =>
            {
                var jwt = configuration.GetRequiredSection(nameof(JwtConfiguration)).Get<JwtConfiguration>();
                configure.SaveToken = jwt!.SaveToken;
                configure.Audience = jwt.Audience;
                configure.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidIssuer = jwt.ValidIssuer,
                    ValidateLifetime = true,
                    ValidateTokenReplay = true,
                    ValidateIssuerSigningKey = true,
                    ValidateActor = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey))
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}