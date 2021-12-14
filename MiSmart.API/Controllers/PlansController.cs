

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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq.Expressions;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using MiSmart.DAL.Extensions;

namespace MiSmart.API.Controllers
{
    public class PlansController : AuthorizedAPIControllerBase
    {
        public PlansController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromServices] PlanRepository planRepository, [FromForm] AddingPlanCommand command)
        {
            var response = actionResponseFactory.CreateInstance();


            var plan = planRepository.Get(ww => ww.FileName == command.File.FileName && ww.Prefix == command.Prefix);
            if (plan is null)
            {
                plan = new Plan { FileName = command.File.FileName, Prefix = command.Prefix };
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            plan.Location = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            plan.FileName = command.File.FileName;
            plan.FileBytes = command.GetFileBytes();

            if (plan.ID == 0)
            {
                planRepository.Create(plan);
            }
            else
            {
                planRepository.Update(plan);
            }
            response.SetCreatedObject(plan);



            return response.ToIActionResult();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetList([FromServices] PlanRepository planRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search,
        [FromQuery] Double? latitude, [FromQuery] Double? longitude, [FromQuery] Double? range)
        {
            var response = actionResponseFactory.CreateInstance();
            Point centerLocation = null;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            if (latitude.HasValue && longitude.HasValue && range.HasValue)
            {
                centerLocation = geometryFactory.CreatePoint(new Coordinate(longitude.GetValueOrDefault(), latitude.GetValueOrDefault()));
            }
            Expression<Func<Plan, Boolean>> query = ww => (String.IsNullOrWhiteSpace(search) ?
            ((centerLocation != null) ? (ww.Location.Distance(centerLocation) < range.GetValueOrDefault()) : true)
            : ww.FileName.ToLower().Contains(search.ToLower()));
            var listResponse = planRepository.GetListResponseView<SmallPlanViewModel>(pageCommand, query);
            if (centerLocation is not null)
            {
                foreach (var item in listResponse.Data)
                {
                    item.CalculateDistance(centerLocation);
                }
            }
            listResponse.SetResponse(response);

            return response.ToIActionResult();

        }

        [AllowAnonymous]
        [HttpGet("{id:long}/GetFile")]
        public IActionResult GetFile([FromServices] PlanRepository planRepository, [FromRoute] Int64 id)
        {
            var response = actionResponseFactory.CreateInstance();

            var plan = planRepository.Get(ww => ww.ID == id);
            if (plan is null)
            {

                response.AddNotFoundErr("Plan");
            }

            response.SetFile(plan.FileBytes, "application/octet-stream", plan.FileName);



            return response.ToIActionResult();
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete([FromServices] PlanRepository planRepository, [FromRoute] Int64 id)
        {
            var response = actionResponseFactory.CreateInstance();

            var plan = planRepository.Get(ww => ww.ID == id);
            if (plan is null)
            {
                response.AddNotFoundErr("Plan");
            }

            planRepository.Delete(plan);
            response.SetNoContent();



            return response.ToIActionResult();
        }
    }
}