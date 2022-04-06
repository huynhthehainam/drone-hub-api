


using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.ViewModels;
using MiSmart.DAL.ViewModels;

namespace MiSmart.API.Controllers
{
    public class ExecutionCompanySettingsController : AuthorizedAPIControllerBase
    {
        public ExecutionCompanySettingsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        
        [HttpPost]
        public IActionResult CreateSetting([FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository,
             [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] AddingExecutionCompanySettingCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var setting = new ExecutionCompanySetting
            {
                CostPerHectare = command.CostPerHectare.GetValueOrDefault(),
                CreatedTime = DateTime.Now,
                ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID,
                MainPilotCostPerHectare = command.MainPilotCostPerHectare.GetValueOrDefault(),
                SubPitlotCostPerHectare = command.SubPitlotCostPerHectare.GetValueOrDefault(),
            };
            executionCompanySettingRepository.Create(setting);

            response.SetCreatedObject(setting);
            return response.ToIActionResult();
        }
        [HttpGet("Latest")]
        public IActionResult GetLatestSetting([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository)
        {
            var response = actionResponseFactory.CreateInstance();

            var executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var latestSetting = executionCompanySettingRepository.GetLatestSetting(executionCompanyUser.ExecutionCompanyID);

            var vm = ViewModelHelpers.ConvertToViewModel<ExecutionCompanySetting, ExecutionCompanySettingViewModel>(latestSetting);

            response.SetData(vm);


            return response.ToIActionResult();
        }
    }
}