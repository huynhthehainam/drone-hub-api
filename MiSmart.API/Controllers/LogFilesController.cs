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
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;

namespace MiSmart.API.Controllers
{
    public class LogFilesController : AuthorizedAPIControllerBase
    {
        public LogFilesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromQuery] Int32? deviceID, [FromServices] LogFileRepository logFileRepository, 
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            Expression<Func<LogFile, Boolean>> query = ww => 
            (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true) 
            && (ww.FileBytes.Length > 0 ? (ww.FileBytes[0] > 0) : false);

            var listResponse = await logFileRepository.GetListResponseViewAsync<LogFileViewModel>(pageCommand, query, ww => ww.LoggingTime, false);

            listResponse.SetResponse(actionResponse);
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/File")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> GetFile([FromRoute] Guid id, [FromServices] LogFileRepository logFileRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
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