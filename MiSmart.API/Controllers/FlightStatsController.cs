
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using MiSmart.Infrastructure.Helpers;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.API.Controllers
{
    public class FlightStatsController : AuthorizedAPIControllerBase
    {
        public FlightStatsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("{id:Guid}")]
        public IActionResult GetByID([FromServices] FlightStatRepository flightStatRepository, [FromRoute] Guid id)
        {
            var response = actionResponseFactory.CreateInstance();
            var flightStat = flightStatRepository.Get(ww => ww.ID == id);
            var validated = true;
            if (flightStat is null)
            {
                validated = false;
                response.AddNotFoundErr("FlightStat");
            }

            if (validated)
            {
                response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));

            }

            return response.ToIActionResult();
        }
    }
}