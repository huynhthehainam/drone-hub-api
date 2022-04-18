


using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.ViewModels;
using MiSmart.DAL.ViewModels;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class ExecutionCompanySettingsController : AuthorizedAPIControllerBase
    {
        public ExecutionCompanySettingsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting([FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository,
             [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] AddingExecutionCompanySettingCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var setting = await executionCompanySettingRepository.CreateAsync(new ExecutionCompanySetting
            {
                CostPerHectare = command.CostPerHectare.GetValueOrDefault(),
                CreatedTime = DateTime.UtcNow,
                ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID,
                MainPilotCostPerHectare = command.MainPilotCostPerHectare.GetValueOrDefault(),
                SubPilotCostPerHectare = command.SubPilotCostPerHectare.GetValueOrDefault(),
            });

            response.SetCreatedObject(setting);
            return response.ToIActionResult();
        }
        [HttpGet("Latest")]
        public async Task<IActionResult> GetLatestSetting([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository)
        {
            var response = actionResponseFactory.CreateInstance();

            var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var latestSetting = executionCompanySettingRepository.GetLatestSetting(executionCompanyUser.ExecutionCompanyID);
            if (latestSetting is null)
            {
                response.AddNotFoundErr("LatestSetting");
            }
            var vm = ViewModelHelpers.ConvertToViewModel<ExecutionCompanySetting, ExecutionCompanySettingViewModel>(latestSetting);

            response.SetData(vm);


            return response.ToIActionResult();
        }
    }
}