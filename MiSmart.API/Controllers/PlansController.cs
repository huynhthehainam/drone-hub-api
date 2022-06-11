



using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.DatabaseContexts;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MiSmart.API.Controllers
{
    public class PlansController : AuthorizedAPIControllerBase
    {
        public PlansController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromServices] PlanRepository planRepository,
        [FromQuery] String search, [FromServices] DatabaseContext databaseContext,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromQuery] String relation = "Executor")
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            Expression<Func<Plan, Boolean>> query = ww => false;
            if (relation == "Executor")
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID, ExecutionCompanyUserType.Owner);
                if (executionCompanyUser is null)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => true
                && String.IsNullOrEmpty(search) ? true : ww.FileName.ToLower().Contains(search.ToLower());

            }
            var listResponse = await planRepository.GetListResponseViewAsync<SmallPlanViewModel>(pageCommand, query);
            
            listResponse.SetResponse(actionResponse);
            return actionResponse.ToIActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddingPlanCommand command, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] PlanRepository planRepository,

        [FromServices] DeviceRepository deviceRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                actionResponse.AddNotAllowedErr();
            }

            var device = await deviceRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (device is null)
            {
                actionResponse.AddNotAllowedErr();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var plan = await planRepository.GetAsync(ww => ww.FileName == command.File.FileName && ww.Device == device);
            if (plan is null)
            {
                plan = new Plan { FileName = command.File.FileName, Device = device };
            }

            plan.Location = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            plan.FileName = command.File.FileName;
            plan.Area = command.Area.GetValueOrDefault();
            plan.FileBytes = command.GetFileBytes();

            if (plan.ID == 0)
            {
                await planRepository.CreateAsync(plan);
            }
            else
            {
                await planRepository.UpdateAsync(plan);
            }
            actionResponse.SetCreatedObject(plan);

            return actionResponse.ToIActionResult();
        }
    }
}