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
using System.IO;
using System.IO.Compression;
using System.Linq;

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
            && (ww.FileBytes.Length > 500000);

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
        [HttpGet("GetZipFile")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> GetZipFile([FromServices] LogFileRepository logFileRepository, [FromQuery] PageCommand pageCommand, [FromQuery] Int32? deviceID, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            Expression<Func<LogFile, Boolean>> query = ww => (ww.FileBytes.Length >= 500000) && (deviceID.HasValue ? ww.DeviceID == deviceID.GetValueOrDefault() : true) && (from.HasValue ? ww.LoggingTime >= from.GetValueOrDefault() : true) && (to.HasValue ? ww.LoggingTime <= to.GetValueOrDefault() : true);
            var data = await logFileRepository.GetListEntitiesAsync(pageCommand, query);
            var groupedData = data.GroupBy(ww => ww.Device);
            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create))
                {
                    foreach (var item in groupedData)
                    {
                        foreach (var fileItem in item)
                        {
                            var path = $"{item.Key.Name}_{item.Key.ID}/{fileItem.FileName}";
                            var fileEntry = archive.CreateEntry(path);

                            using (var entryStream = fileEntry.Open())
                            {
                                var fileBytes = fileItem.FileBytes;
                                entryStream.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                    }
                }
                var compressBytes = ms.ToArray();
                actionResponse.SetFile(compressBytes, "application/zip", "compress.zip");
            }


            return actionResponse.ToIActionResult();
        }

    }
}