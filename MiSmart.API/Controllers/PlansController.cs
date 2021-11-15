

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
            var validated = true;

            var plan = planRepository.Get(ww => ww.Name == command.Name);
            if (plan is null)
            {
                plan = new Plan { Name = command.Name, };
            }
            plan.Latitude = command.Latitude.GetValueOrDefault();
            plan.Longitude = command.Longitude.GetValueOrDefault();
            plan.FileName = command.File.FileName;
            plan.FileBytes = command.GetFileBytes();
            if (validated)
            {
                if (plan.ID == 0)
                {
                    planRepository.Create(plan);
                }
                else
                {
                    planRepository.Update(plan);
                }
            }

            return response.ToIActionResult();
        }

        [AllowAnonymous]
        [HttpGet("{id:long}/GetFile")]
        public IActionResult GetFile([FromServices] PlanRepository planRepository, [FromRoute] Int64 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            var plan = planRepository.Get(ww => ww.ID == id);
            if (plan is null)
            {
                validated = false;

            }
            if (validated)
            {
                response.SetFile(plan.FileBytes, "application/octet-stream", plan.FileName);
            }


            return response.ToIActionResult();
        }
    }
}