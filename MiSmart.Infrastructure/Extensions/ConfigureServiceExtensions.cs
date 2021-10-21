using System.Text;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Linq;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.Infrastructure.Extensions
{
    public static class ConfigureServiceExtensions
    {
        public static void ConfigureJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authSettingsSection = configuration.GetSection("AuthSettings");
            services.Configure<AuthSettings>(authSettingsSection);
            var appSettings = authSettingsSection.Get<AuthSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.AuthSecret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents();
                x.Events.OnMessageReceived = (context) =>
                {
                    var bb = context.HttpContext.Request.Query["access_token"];
                    context.Token = context.HttpContext.Request.Query["access_token"];
                    return Task.CompletedTask;
                };

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}