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
using MiSmart.Infrastructure.ViewModels;
using MiSmart.API.Commands;
using MiSmart.Infrastructure.Minio;
using System.Collections.Generic;
using MiSmart.API.Services;
using MiSmart.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace MiSmart.API.Controllers
{
    class UserEmail{
        public UserEmail(String Email, Guid UUID){
            this.Email = Email;
            this.UUID = UUID;
        }
        public String Email {get; set;}
        public Guid UUID {get; set;}
    }
    public class LogFilesController : AuthorizedAPIControllerBase
    {
        private List<UserEmail> listEmailForLog = new List<UserEmail>{
            new UserEmail("dotientrung201030@gmail.com",new Guid("0c1fb569-05f0-4ccb-b439-1dbed807f38d")),
        };
        private List<UserEmail> listEmailForError = new List<UserEmail>{
            new UserEmail("dotientrung201030@gmail.com",new Guid("0c1fb569-05f0-4ccb-b439-1dbed807f38d")),
        };
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
            if (data.Count == 0)
            {
                actionResponse.AddNotFoundErr("ZipFile");
            }
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
        [HttpGet("{id:Guid}/Detail")]
        public async Task<IActionResult> GetLogDetail([FromRoute] Guid id, [FromServices] LogDetailRepository logDetailRepository){
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == id);
             if (logDetail is null)
            {
                actionResponse.AddNotFoundErr("LogDetail");
            }
            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogDetail, LogDetailViewModel>(logDetail));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/Report")]
        public async Task<IActionResult> GetLogReport([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository){
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }
            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogReport, LogReportViewModel>(logReport));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/Result")]
        public async Task<IActionResult> GetLogResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository){
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReportResult is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }
            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(logReportResult));
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/Report")]
        public async Task<IActionResult> CreateReport([FromRoute] Guid id, 
        [FromForm] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository, 
        [FromServices] LogFileRepository logFileRepository, [FromServices] MinioService minioService,  
        [FromServices] MyEmailService emailService, [FromServices] LogTokenRepository logTokenRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null){
                response.AddNotFoundErr("LogFile");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            var report = await logReportRepository.CreateAsync(new LogReport() 
            {
                LogFileID = id,
                AccidentTime = command.AccidentTime, 
                ImageUrls = imageUrls.ToArray(),
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                UserUUID = CurrentUser.UUID, 
            });
            
            response.SetCreatedObject(report);
            
            foreach(UserEmail item in listEmailForLog){
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken(){Token = token, UserUUID = item.UUID, LogFileID = id});
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích] Mã hiệu drone ({logFile.Device.Name})", @$"
                Dear,
                Phòng Đặc Nhiệm trả kết quả báo cáo hiện tường:
                Mã hiệu Drone: {logFile.Device.Name}
                Link Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/log-report?token={token}");
            }

            return response.ToIActionResult();
        }
        [HttpPatch("{id:Guid}/Report")]
        public async Task<IActionResult> UpdateReport([FromRoute] Guid id, [FromForm] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository, [FromServices] LogFileRepository logFileRepository, [FromServices] MinioService minioService){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null){
                response.AddNotFoundErr("LogReport");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            
            logReport.UpdatedTime = new DateTime();
            logReport.AccidentTime = command.AccidentTime;
            if (imageUrls.LongCount() != 0){
                logReport.ImageUrls = imageUrls.ToArray();
            }
            logReport.PilotDescription = command.PilotDescription;
            logReport.ReporterDescription = command.ReporterDescription;
            await logReportRepository.UpdateAsync(logReport);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/Result")]
        public async Task<IActionResult> CreateReportResult([FromRoute] Guid id, [FromForm] AddingLogResultCommand command, [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogFileRepository logFileRepository, [FromServices] MinioService minioService){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logFile = logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null){
                response.AddNotFoundErr("LogFile");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            var report = await logReportResultRepository.CreateAsync(new LogReportResult() 
            {
                ImageUrls = imageUrls.ToArray(),
                ExecutionCompanyID = command.ExecutionCompanyID,
                DetailedAnalysis = command.DetailedAnalysis,
                LogFileID = id,
                AnalystUUID = CurrentUser.UUID,
                LogResultDetails = command.ListErrors,
                Suggest = command.Suggest,
                Conclusion = command.Conclusion,
            });
            foreach(UserEmail item in listEmailForLog){
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken(){Token = token, UserUUID = item.UUID, LogFileID = id});
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})", @$"
                Dear,
                Phòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:
                Mã hiệu Drone: {logFile.Device.Name}
                Kết luận chung: {command.Conclusion}");
            }
            response.SetCreatedObject(report);
            return response.ToIActionResult();
        }
        [HttpPatch("{id:Guid}/Result")]
        public async Task<IActionResult> UpdateReportResult([FromRoute] Guid id, [FromForm] AddingLogResultCommand command, [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MinioService minioService){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null){
                response.AddNotFoundErr("LogResult");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            logResult.ImageUrls = imageUrls.ToArray();
            logResult.ExecutionCompanyID = command.ExecutionCompanyID;
            logResult.DetailedAnalysis = command.DetailedAnalysis;
            logResult.LogFileID = id;
            logResult.LogResultDetails = command.ListErrors;
            logResult.AnalystUUID = CurrentUser.UUID;
            logResult.Suggest = command.Suggest;
            logResult.Conclusion = command.Conclusion;

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/ApprovedResult")]
        public async Task<IActionResult> ApprovedResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null){
                response.AddNotFoundErr("LogResult");
            }
            logResult.ApproverUUID = CurrentUser.UUID;
            
            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpPost("ResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateResultFromEmail([FromForm] AddingLogResultFromMailCommand command,
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository,
        [FromServices] LogReportResultRepository logReportResultRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            var result = await logReportResultRepository.CreateAsync(new LogReportResult() 
            {
                AnalystUUID = token.UserUUID,
                Conclusion = command.Conclusion,
                DetailedAnalysis = command.DetailedAnalysis,
                ExecutionCompanyID = command.ExecutionCompanyID,
                ImageUrls = imageUrls.ToArray(),
                LogFileID = token.LogFileID,
                Suggest = command.Suggest,
                LogResultDetails = command.ListErrors
            });
            response.SetCreatedObject(result);
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.Token == command.Token);
            await tokenRepository.DeleteRangeAsync(listLogToken);
            return response.ToIActionResult();
        }
        [HttpPost("ReportFromEmail")]
        public async Task<IActionResult> CreateReportFromEmail( 
        [FromForm] AddingLogReportFromEmailCommand command, [FromServices] LogReportRepository logReportRepository, 
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var imageUrls = new List<String>();
            foreach(var file in command.Files){
                imageUrls.Add(await minioService.PutFileAsync(file, new String[] { "drone-hub-api", "log-reports" }));
            }
            var report = await logReportRepository.CreateAsync(new LogReport() 
            {
                LogFileID = token.LogFileID,
                AccidentTime = command.AccidentTime, 
                ImageUrls = imageUrls.ToArray(),
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                UserUUID = token.UserUUID, 
            });
            
            response.SetCreatedObject(report);
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.Token == command.Token);
            await tokenRepository.DeleteRangeAsync(listLogToken);
            return response.ToIActionResult();
        }
        [HttpGet("DetailForEmail")]
        public async Task<IActionResult> GetLogDetailForEmail([FromServices] LogDetailRepository logDetailRepository,
        [FromServices] LogTokenRepository tokenRepository, [FromBody] AddingLogDetailForEmailCommand command){
           ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
             if (logDetail is null)
            {
                response.AddNotFoundErr("LogDetail");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogDetail, LogDetailViewModel>(logDetail));
            return response.ToIActionResult();
        }
        // [HttpPost("Errors")]
        // public Task<IActionResult> SendEmailErrors([FromBody] AddingLogErrorCommand command){
        //     ActionResponse response = actionResponseFactory.CreateInstance();

        //     return Task.FromResult(response.ToIActionResult());
        // }
    }
}