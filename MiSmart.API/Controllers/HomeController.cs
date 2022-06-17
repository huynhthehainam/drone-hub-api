using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public Task<IActionResult> Index()
        {
            var response = actionResponseFactory.CreateInstance();
            response.SetData(new
            {
                Version = "1.0.0",
                CreatedBy = "MiSmart",
                Service = "App Sync",
                Description = "MiSmart is the best drone company in VN",
                AllowedVersions = Constants.AllowedVersions,
            });
            return Task.FromResult(response.ToIActionResult());
        }
    }
}