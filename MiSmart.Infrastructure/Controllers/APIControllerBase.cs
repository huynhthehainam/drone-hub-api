using Microsoft.AspNetCore.Mvc;
using MiSmart.Infrastructure.Responses;
using System;
namespace MiSmart.Infrastructure.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class APIControllerBase : Controller
    {
        protected IActionResponseFactory actionResponseFactory;
        protected APIControllerBase(IActionResponseFactory actionResponseFactory)
        {
            this.actionResponseFactory = actionResponseFactory;
        }
        public String RequestIP
        {
            get
            {
                return HttpContext.Connection.RemoteIpAddress.ToString();
            }
        }
    }
}