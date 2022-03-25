

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.Infrastructure.Commands;
using System.Linq.Expressions;
using System;
using MiSmart.DAL.ViewModels;

namespace MiSmart.API.Controllers
{
    public class BatteriesController : AuthorizedAPIControllerBase
    {
        public BatteriesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }


        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingBatteryCommand command,
        [FromServices] BatteryModelRepository batteryModelRepository,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository,
         [FromServices] BatteryRepository batteryRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            if (command.ExecutionCompanyID.HasValue)
            {
                ExecutionCompany executionCompany = executionCompanyRepository.Get(c => c.ID == command.ExecutionCompanyID.GetValueOrDefault());
                if (executionCompany is null)
                {
                    response.AddInvalidErr("ExecutionCompanyID");
                }
            }
            BatteryModel batteryModel = batteryModelRepository.Get(ww => ww.ID == command.BatteryModelID.GetValueOrDefault());
            if (batteryModel is null)
            {
                response.AddInvalidErr("BatteryModelID");
            }
            Battery battery = new Battery { ActualID = command.ActualID, ExecutionCompanyID = command.ExecutionCompanyID, BatteryModel = batteryModel };

            batteryRepository.Create(battery);
            response.SetCreatedObject(battery);
            return response.ToIActionResult();
        }

        [HttpGet]
        public IActionResult GetList([FromQuery] PageCommand pageCommand, [FromServices] BatteryRepository batteryRepository, [FromQuery] String search, [FromQuery] String relation = "Administrator")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Battery, Boolean>> query = b => false;
            if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdmin && CurrentUser.RoleID != 1)
                {
                    response.AddNotAllowedErr();
                }
                query = b => true;
            }

            var listResponse = batteryRepository.GetListResponseView<BatteryViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
    }
}