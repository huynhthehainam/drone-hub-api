using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Commands;

namespace MiSmart.API.Controllers
{
    public class LogFilesController : AuthorizedAPIControllerBase
    {
        public LogFilesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromServices] LogFileRepository logFileRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
            if (executionCompanyUser is null)
            {
                actionResponse.AddNotAllowedErr();
            }
            var listResponse = await logFileRepository.GetListResponseViewAsync<LogFileViewModel>(pageCommand, ww => true, ww => ww.LoggingTime, false);

            listResponse.SetResponse(actionResponse);
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/File")]
        public async Task<IActionResult> GetFile([FromRoute] Guid id, [FromServices] LogFileRepository logFileRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
            if (executionCompanyUser is null)
            {
                actionResponse.AddNotAllowedErr();
            }
            Expression<Func<LogFile, Boolean>> query = ww => (ww.ID == id);
            var logFile = await logFileRepository.GetAsync(query);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
            }

            actionResponse.SetFile(logFile.FileBytes, "application/octet-stream", logFile.FileName);
            return actionResponse.ToIActionResult();
        }

    }
}