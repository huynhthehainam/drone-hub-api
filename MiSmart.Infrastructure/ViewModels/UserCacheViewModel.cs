using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Data;
namespace MiSmart.Infrastructure.ViewModels
{
    public class UserCacheViewModel
    {
        public String Username { get; set; }
        public String Email { get; set; }
        public Boolean IsAdmin { get; set; }
        public Int32 RoleID { get; set; }
        public Int64 ID { get; set; }
        public Boolean IsActive { get; set; }
        public List<Int32> AllowedFunctionIds { get; set; }
        public UserCacheViewModel() { }
        public static UserCacheViewModel GetUserCache(ClaimsPrincipal userClaims)
        {
            UserCacheViewModel user = null;
            if (userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim) != null)
            {
                user = JsonSerializer.Deserialize<UserCacheViewModel>(userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim).Value);
            }
            return user;
        }
    }
}