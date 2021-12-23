using System.Linq;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.Controllers;
using MiSmart.API.Models;

namespace MiSmart.API.ControllerBases
{
    [Authorize]
    public abstract class AuthorizedDeviceAPIControllerBase : APIControllerBase
    {
        protected AuthorizedDeviceAPIControllerBase(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        public AuthorizedDeviceModel CurrentDevice
        {
            get
            {
                var userClaims = HttpContext.User;
                if (userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim) != null)
                {
                    return JsonSerializer.Deserialize<AuthorizedDeviceModel>(userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.JWTAuthKey).Value);
                }
                return null;
            }
        }
    }

}