using System;
using System.Linq;
using MiSmart.API.Commands;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Helpers;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.Settings;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiSmart.DAL.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.RegularExpressions;
using MiSmart.Infrastructure.Commands;
using System.Threading.Tasks;
using MiSmart.API.Services;
using System.IdentityModel.Tokens.Jwt;

namespace MiSmart.API.Controllers
{
    public class AuthController : APIControllerBase
    {
        public AuthController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost("GenerateDeviceToken")]
        public IActionResult GenerateDeviceAccessToken([FromServices] IOptions<AuthSettings> options, [FromServices] DeviceRepository deviceRepository, [FromBody] GeneratingDeviceAccessTokenCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.Token == command.DeviceToken);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            if (device.AccessToken is null || device.NextGeneratingAccessTokenTime.GetValueOrDefault() < DateTime.Now)
            {
                device.AccessToken = device.GenerateDeviceAccessToken(options.Value.AuthSecret);
                device.NextGeneratingAccessTokenTime = DateTime.Now.AddMonths(1);
                deviceRepository.Update(device);
            }


            response.SetData(new { AccessToken = device.AccessToken });

            return response.ToIActionResult();
        }
    }
}