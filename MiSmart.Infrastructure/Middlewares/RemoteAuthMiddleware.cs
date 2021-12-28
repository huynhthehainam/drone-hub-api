using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using MiSmart.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
namespace MiSmart.Infrastructure.Middlewares
{
    public class RemoteAuthMiddleware
    {
        private readonly RequestDelegate next;

        public RemoteAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, JWTService jwtService)
        {
            var cc = context.Request.Headers["Connection"];
            if (context.Request.Headers["Connection"] == "Upgrade")
            {
                context.Request.Query.TryGetValue("access_token", out var token);
                if (token.Count > 0)
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token[0]);
                }
                else
                {
                    context.Response.StatusCode = 401;
                }
            }
            String authHeader = context.Request.Headers[Keys.AuthHeaderKey];
            if (authHeader != null)
            {
                authHeader = authHeader.Replace(Keys.JWTPrefixKey, "").Trim();
                var validator = new JwtSecurityTokenHandler();
                if (validator.CanReadToken(authHeader))
                {
                    Int64? userID = jwtService.GetUserID(authHeader);
                    var isAdmin = jwtService.IsUserAdmin(authHeader);
                    if (!userID.HasValue)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    var user = new UserCacheViewModel
                    {
                        ID = userID.Value,
                        IsAdmin = isAdmin,
                    };
                    ClaimsIdentity aa = new ClaimsIdentity();
                    var claims = new[]{
                        new Claim(Keys.IdentityClaim,JsonSerializer.Serialize(user))
                    };
                    var identity = new ClaimsIdentity(claims, "basic");
                    context.User = new ClaimsPrincipal(identity);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            await next(context);
        }
    }
}