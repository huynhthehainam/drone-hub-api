using System.Linq;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.Controllers;

namespace MiSmart.API.ControllerBases
{
    [Authorize]
    public abstract class AuthorizedDeviceAPIControllerBase : APIControllerBase
    {
        protected AuthorizedDeviceAPIControllerBase(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        public UserCacheViewModel CurrentDevice
        {
            get
            {
                var userClaims = HttpContext.User;
                if (userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim) != null)
                {
                    var claimsString = userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim).Value;
                    var vm = JsonSerializer.Deserialize<UserCacheViewModel>(claimsString);
                    if (vm.Type == "Device")
                        return vm;
                }
                ActionResponse actionResponse = actionResponseFactory.CreateInstance();
                actionResponse.AddAuthorizationErr();
                return null;
            }
        }
    }

}