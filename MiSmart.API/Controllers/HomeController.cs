using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MiSmart.API.Services;

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
        public Task<IActionResult> Index([FromServices] CountingService countingService)
        {
            var response = actionResponseFactory.CreateInstance();
            response.SetData(new
            {
                Version = "1.0.4",
                CreatedBy = "MiSmart",
                Service = "App Sync",
                Description = "MiSmart is the best drone company in VN",
                AllowedVersions = Constants.AllowedVersions,
                Count = countingService.Count,
                Count2 = countingService.Count2,
                Count3 = countingService.Count3,
                Count4 = countingService.Count4,
                Count5 = countingService.Count5,
                Heartbeat = countingService.Heartbeat,
            });
            return Task.FromResult(response.ToIActionResult());
        }
    }
}