using System.Linq;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.Controllers;
using System;

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
                var claim = userClaims.Claims.FirstOrDefault(ww => ww.Type == Keys.IdentityClaim);
                if (claim != null)
                {
                    var claimsString = claim.Value;
                    var vm = JsonSerializer.Deserialize<UserCacheViewModel>(claimsString);
                    if (vm is not null && vm.Type == "Device")
                        return vm;
                }
                ActionResponse actionResponse = actionResponseFactory.CreateInstance();
                actionResponse.AddAuthorizationErr();
                throw new Exception("Cannot found current device");
            }
        }
    }

}