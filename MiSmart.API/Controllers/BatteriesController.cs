

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
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class BatteriesController : AuthorizedAPIControllerBase
    {
        public BatteriesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] Int32 id, [FromBody] PatchingBatteryCommand command, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] BatteryRepository batteryRepository, [FromServices] BatteryModelRepository batteryModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var battery = await batteryRepository.GetAsync(ww => ww.ID == id);
            if (battery is null)
            {
                response.AddNotFoundErr("Battery");
                return response.ToIActionResult();
            }

            if (command.BatteryModelID.HasValue)
            {
                var batteryModel = await batteryModelRepository.GetAsync(ww => ww.ID == command.BatteryModelID.GetValueOrDefault());
                if (batteryModel is null)
                {
                    response.AddInvalidErr("BatteryModelID");
                    return response.ToIActionResult();
                }
                battery.BatteryModel = batteryModel;
            }
            if (command.ExecutionCompanyID.HasValue)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == command.ExecutionCompanyID.GetValueOrDefault());
                if (executionCompany is null)
                {
                    response.AddInvalidErr("ExecutionCompanyID");
                    return response.ToIActionResult();
                }
                battery.ExecutionCompany = executionCompany;
            }

            battery.Name = String.IsNullOrEmpty(command.Name) ? battery.Name : command.Name;


            await batteryRepository.UpdateAsync(battery);
            response.SetUpdatedMessage();


            return response.ToIActionResult();
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> Create([FromBody] AddingBatteryCommand command,
        [FromServices] BatteryModelRepository batteryModelRepository,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository,
         [FromServices] BatteryRepository batteryRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            if (command.ExecutionCompanyID.HasValue)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(c => c.ID == command.ExecutionCompanyID.GetValueOrDefault());
                if (executionCompany is null)
                {
                    response.AddInvalidErr("ExecutionCompanyID");
                    return response.ToIActionResult();
                }
            }
            var batteryModel = await batteryModelRepository.GetAsync(ww => ww.ID == command.BatteryModelID.GetValueOrDefault());
            if (batteryModel is null)
            {
                response.AddInvalidErr("BatteryModelID");
                return response.ToIActionResult();
            }
            Battery battery = new Battery { ActualID = command.ActualID, ExecutionCompanyID = command.ExecutionCompanyID, BatteryModel = batteryModel, Name = command.Name };

            await batteryRepository.CreateAsync(battery);
            response.SetCreatedObject(battery);
            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromServices] BatteryRepository batteryRepository, [FromQuery] String? search, [FromQuery] String? relation = "Administrator")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Battery, Boolean>> query = b => false;
            if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }
                query = b => String.IsNullOrEmpty(search) ? true : ((b.Name ?? "").ToLower().Contains(search.ToLower()) || (b.ActualID ?? "").ToLower().Contains(search.ToLower()));
            }
            else if (relation == "Maintainer")
            {
                if (CurrentUser.RoleID != 3)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }
                query = b => String.IsNullOrEmpty(search) ? true : ((b.Name ?? "").ToLower().Contains(search.ToLower()) || (b.ActualID ?? "").ToLower().Contains(search.ToLower()));
            }

            var listResponse = await batteryRepository.GetListResponseViewAsync<BatteryViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
    }
}