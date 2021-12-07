
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
            var flightStat = flightStatRepository.GetView<LargeFlightStatViewModel>(ww => ww.ID == id);

            response.SetData(flightStat);

            return response.ToIActionResult();
        }
    }
}