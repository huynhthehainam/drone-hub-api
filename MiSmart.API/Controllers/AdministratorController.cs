using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Commands;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;

namespace MiSmart.API.Controllers
{
    public class AdministratorController : AuthorizedAPIControllerBase
    {
        public AdministratorController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }


        [HttpGet("FlightStats")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetFlightStats([FromServices] FlightStatRepository flightStatRepository, [FromQuery] PageCommand pageCommand)
        {
            var response = actionResponseFactory.CreateInstance();

            var listResponse = flightStatRepository.GetListFlightStatsView<SmallFlightStatViewModel>(pageCommand, ww => true, ww => ww.CreatedTime, false);

            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }


        [HttpGet("Devices")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetDevices([FromQuery] PageCommand pageCommand, [FromServices] DeviceRepository deviceRepository)
        {
            var response = actionResponseFactory.CreateInstance();


            var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, ww => true);
            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }

    }
}