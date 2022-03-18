
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;

namespace MiSmart.API.Controllers
{
    public class SettingsController : AuthorizedAPIControllerBase
    {
        public SettingsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("ConversionSettings")]
        public IActionResult GetConversionSettings([FromServices] IOptions<ConversionSettings> options)
        {
            var response = actionResponseFactory.CreateInstance();
            ConversionSettings settings = options.Value;
            response.SetData(settings);
            return response.ToIActionResult();
        }
    }
}