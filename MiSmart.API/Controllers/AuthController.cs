using System;
using MiSmart.API.Commands;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class AuthController : APIControllerBase
    {
        public AuthController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost("GenerateDeviceToken")]
        public async Task<IActionResult> GenerateDeviceAccessToken([FromServices] IOptions<AuthSettings> options, [FromServices] DeviceRepository deviceRepository, [FromBody] GeneratingDeviceAccessTokenCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.Token == command.DeviceToken);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            if (device.AccessToken is null || device.NextGeneratingAccessTokenTime.GetValueOrDefault() < DateTime.UtcNow.AddDays(2))
            {
                device.AccessToken = device.GenerateDeviceAccessToken(options.Value.AuthSecret);
                device.NextGeneratingAccessTokenTime = DateTime.UtcNow.AddMonths(1);
                await deviceRepository.UpdateAsync(device);
            }


            response.SetData(new { AccessToken = device.AccessToken });

            return response.ToIActionResult();
        }
    }
}