
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class SettingsController : AuthorizedAPIControllerBase
    {
        public SettingsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("ConversionSettings")]
        public Task<IActionResult> GetConversionSettings([FromServices] IOptions<ConversionSettings> options)
        {
            var response = actionResponseFactory.CreateInstance();
            ConversionSettings settings = options.Value;
            response.SetData(settings);
            return Task.FromResult(response.ToIActionResult());
        }
    }
}