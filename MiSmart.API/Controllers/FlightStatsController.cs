
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;
using System.Linq.Expressions;
using MiSmart.DAL.Responses;

namespace MiSmart.API.Controllers
{
    public class FlightStatsController : AuthorizedAPIControllerBase
    {
        public FlightStatsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public IActionResult GetFlightStats([FromServices] FlightStatRepository flightStatRepository,
        [FromServices] TeamUserRepository teamUserRepository,
        [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Int64? teamID, [FromQuery] Int32? deviceID, [FromQuery] Int32? deviceModelID, [FromQuery] String mode = "Small")
        {
            var response = new FlightStatsActionResponse();
            response.ApplySettings(actionResponseFactory.Settings);

            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }





            Expression<Func<FlightStat, Boolean>> query = ww => (ww.CustomerID == customerUser.CustomerID)
                && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                && (to.HasValue ? (ww.FlightTime <= to.Value) : true)
                && (true);
            if (mode == "Large")
            {

            }
            else
            {
                var listResponse = flightStatRepository.GetListFlightStatsView<SmallFlightStatViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
                listResponse.SetResponse(response);
            }




            return response.ToIActionResult();
        }

        [HttpGet("{id:Guid}")]
        public IActionResult GetByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Guid id)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }
            var flightStat = flightStatRepository.Get(ww => ww.ID == id);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));
            return response.ToIActionResult();
        }
        [HttpDelete("{id:Guid}")]
        public IActionResult DeleteByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Guid id)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var flightStat = flightStatRepository.Get(ww => ww.ID == id);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            flightStatRepository.Delete(flightStat);
            response.SetNoContent();
            return response.ToIActionResult();
        }
    }
}