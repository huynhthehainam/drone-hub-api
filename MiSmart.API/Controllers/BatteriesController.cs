using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;

namespace MiSmart.API.Controllers
{
    public class BatteriesController : AuthorizedAPIControllerBase
    {
        public BatteriesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
      
    }
}