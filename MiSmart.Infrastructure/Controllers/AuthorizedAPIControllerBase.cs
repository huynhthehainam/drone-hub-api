using System.Linq;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Responses;

namespace MiSmart.Infrastructure.Controllers
{
    [Authorize]
    public abstract class AuthorizedAPIControllerBase : APIControllerBase
    {
        protected AuthorizedAPIControllerBase(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        public UserCacheViewModel CurrentUser
        {
            get
            {
                var userClaims = HttpContext.User;
                if (userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim) != null)
                {
                    return JsonSerializer.Deserialize<UserCacheViewModel>(userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAuthKey).Value);
                }
                return null;
            }
        }
    }
}