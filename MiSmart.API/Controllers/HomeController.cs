using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;

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
        public IActionResult Index()
        {
            var response = actionResponseFactory.CreateInstance();
          
            response.Data = new
            {
                CreatedBy = "MiSmart",
                Service = "App Sync",
                Description = "MiSmart is the best drone company in VN"
            };
            return response.ToIActionResult();
        }
    }
}