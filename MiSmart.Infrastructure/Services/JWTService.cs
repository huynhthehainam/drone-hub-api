using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Security.Claims;
using MiSmart.Infrastructure.Constants;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Settings;
using MiSmart.Infrastructure.ViewModels;
namespace MiSmart.Infrastructure.Services
{
    public class JWTService
    {
        private AuthSettings authSettings;
        public JWTService(IOptions<AuthSettings> options1)
        {
            this.authSettings = options1.Value;
        }
        public Int64? GetUserID(String token)
        {
            var validator = new JwtSecurityTokenHandler();
            var jwtToken = validator.ReadJwtToken(token);
            var userClaim = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAuthKey);
            if (userClaim != null)
            {
                return Convert.ToInt64(userClaim.Value);
            }
            return null;
        }
        public Boolean IsUserAdmin(String token)
        {
            var validator = new JwtSecurityTokenHandler();
            var jwtToken = validator.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAdminKey);
            if (claim is not null)
            {
                return Convert.ToBoolean(claim.Value);
            }
            return false;
        }
        public String GenerateAccessToken(UserCacheViewModel user, DateTime accessTokenExpiration)
        {
            var claims = new[] { new Claim(Keys.JWTAuthKey, user.ID.ToString()), new Claim(Keys.JWTAdminKey, user.IsAdmin.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AuthSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, expires: accessTokenExpiration, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}