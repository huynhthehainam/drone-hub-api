using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MiSmart.API.Services;
using Microsoft.AspNetCore.Hosting;

namespace MiSmart.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActionResponseFactory actionResponseFactory;
        public HomeController(IActionResponseFactory actionResponseFactory)
        {
            this.actionResponseFactory = actionResponseFactory;
        }
        [HttpGet]
        public Task<IActionResult> Index([FromServices] CountingService countingService, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var response = actionResponseFactory.CreateInstance();
            response.SetData(new
            {
                Version = "1.0.4",
                CreatedBy = "MiSmart",
                Env = webHostEnvironment.EnvironmentName,
                Service = "App Sync",
                Description = "MiSmart is the best drone company in VN",
                AllowedVersions = Constants.AllowedVersions,
                Count = countingService.Count,
                Count2 = countingService.Count2,
                Count3 = countingService.Count3,
                Count4 = countingService.Count4,
                Count5 = countingService.Count5,
                Count6 = countingService.Count6,
                Heartbeat = countingService.Heartbeat,
            });
            return Task.FromResult(response.ToIActionResult());
        }
    }
}