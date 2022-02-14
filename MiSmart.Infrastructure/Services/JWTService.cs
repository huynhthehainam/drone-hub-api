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
using System.Text.Json.Serialization;

namespace MiSmart.Infrastructure.Services
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JWTUserType
    {
        User,
        Other
    }
    public class JWTService
    {
        private AuthSettings authSettings;
        public JWTService(IOptions<AuthSettings> options1)
        {
            this.authSettings = options1.Value;
        }
        public UserCacheViewModel GetUser(String token)
        {
            var validator = new JwtSecurityTokenHandler();
            var jwtToken = validator.ReadJwtToken(token);
            var userClaim = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAuthKey);
            var adminClaim = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAdminKey);
            var roleClaim = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTRoleKey);
            var type = jwtToken.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTUserTypeKey);
            JWTUserType jWTUserType = JWTUserType.User;
            try
            {
                var typeString = Convert.ToString(type.Value);
                Enum.TryParse<JWTUserType>(typeString, true, out jWTUserType);
            }
            catch (Exception) { }
            if (jWTUserType == JWTUserType.User)
            {
                if (userClaim is not null && adminClaim is not null && roleClaim is not null)
                {

                    return new UserCacheViewModel
                    {
                        ID = Convert.ToInt64(userClaim.Value),
                        RoleID = Convert.ToInt32(roleClaim.Value),
                        IsAdmin = Convert.ToBoolean(adminClaim.Value),
                        IsActive = true,
                    };
                }
            }
            else
            {
                if (userClaim is not null)
                {
                    return new UserCacheViewModel
                    {
                        ID = Convert.ToInt64(userClaim.Value),
                        IsActive = true,
                    };
                }
            }
            return null;
        }
        public String GenerateAccessToken(UserCacheViewModel user, DateTime accessTokenExpiration)
        {
            var claims = new[] {
                new Claim(Keys.JWTAuthKey, user.ID.ToString()),
                new Claim(Keys.JWTAdminKey, user.IsAdmin.ToString()),
                new Claim(Keys.JWTRoleKey, user.RoleID.ToString()),
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AuthSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, expires: accessTokenExpiration, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}