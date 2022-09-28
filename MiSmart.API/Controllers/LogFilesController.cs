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
        private List<String> listEmailForError = new List<String>{
           "dotientrung201030@gmail.com",
        };
        public LogFilesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromQuery] Int32? deviceID, [FromServices] LogFileRepository logFileRepository,
        [FromQuery] Boolean? isStable,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            
            Expression<Func<LogFile, Boolean>> query = ww =>
            (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
            && (ww.FileBytes.Length > 500000)
            && (isStable == false ? (ww.DroneStatus != DroneStatus.Stable) : true);

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
        public async Task<IActionResult> GetLogDetail([FromRoute] Guid id, [FromServices] LogDetailRepository logDetailRepository,
        [FromServices] LogFileRepository logFileRepository){
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null){
                actionResponse.AddNotFoundErr("LogFile");
            }
            if (!logFile.isAnalyzed){
                actionResponse.AddNotFoundErr("LogDetail");
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == id);
            if (logDetail is null)
                actionResponse.SetNoContent();
            else
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
        [FromBody] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository, 
        [FromServices] LogFileRepository logFileRepository, 
        [FromServices] MyEmailService emailService, [FromServices] LogTokenRepository logTokenRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null){
                response.AddNotFoundErr("LogFile");
            }
            var report = new LogReport
            {
                LogFileID = id,
                AccidentTime = command.AccidentTime, 
                ImageUrls = new List<String> { },
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                Suggest = command.Suggest,
                PilotName = command.PilotName,
                PartnerCompanyName = command.PartnerCompanyName,
                UserUUID = CurrentUser.UUID, 
            };
            await logReportRepository.CreateAsync(report);
            
            foreach(UserEmail item in listEmailForLog){
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken {Token = token, UserUUID = item.UUID, LogFileID = id});
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích] Mã hiệu drone ({logFile.Device.Name})", 
                $"Dear,\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\nMã hiệu Drone: {logFile.Device.Name}\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/log-report-result?token={token}");
            }
            response.SetCreatedObject(report);
            return response.ToIActionResult();
        }
        [HttpPatch("{id:Guid}/Report")]
        public async Task<IActionResult> UpdateReport([FromRoute] Guid id, [FromBody] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository, [FromServices] LogFileRepository logFileRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null){
                response.AddNotFoundErr("LogReport");
            }
            logReport.UpdatedTime = DateTime.Now;
            logReport.AccidentTime = command.AccidentTime;
            logReport.Suggest = command.Suggest;
            logReport.ImageUrls = new List<String> { };
            logReport.PartnerCompanyName = command.PartnerCompanyName;
            logReport.PilotDescription = command.PilotDescription;
            logReport.ReporterDescription = command.ReporterDescription;
            await logReportRepository.UpdateAsync(logReport);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/Result")]
        public async Task<IActionResult> CreateReportResult([FromRoute] Guid id, [FromBody] AddingLogResultCommand command, 
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogFileRepository logFileRepository, 
        [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] LogResultDetailRepository logResultDetailRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null){
                response.AddNotFoundErr("LogFile");
            }
            var result = new LogReportResult
            {
                ImageUrls = new List<String>{},
                ExecutionCompanyID = command.ExecutionCompanyID,
                DetailedAnalysis = command.DetailedAnalysis,
                LogFileID = id,
                AnalystUUID = CurrentUser.UUID,
                Suggest = command.Suggest,
                Conclusion = command.Conclusion,
            };
            var res = await logReportResultRepository.CreateAsync(result);

            foreach(AddingLogResultDetailCommand item in command.ListErrors){
                var error = new LogResultDetail{
                    Detail = item.Detail,
                    LogReportResultID = res.ID,
                    PartErrorID = item.PartID,
                    Resolve = item.Resolve,
                    Status = item.Status,
                };
                await logResultDetailRepository.CreateAsync(error);
            }
            
            foreach(UserEmail item in listEmailForLog){
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken(){Token = token, UserUUID = item.UUID, LogFileID = id});
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})", 
                $"Dear,\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\nMã hiệu Drone: {logFile.Device.Name}\nKết luận chung: {command.Conclusion}");
            }
            response.SetCreatedObject(result);
            return response.ToIActionResult();
        }
        
        [HttpPatch("{id:Guid}/Result")]
        public async Task<IActionResult> UpdateReportResult([FromRoute] Guid id, [FromBody] AddingLogResultCommand command, 
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MinioService minioService,
        [FromServices] LogResultDetailRepository logResultDetailRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null){
                response.AddNotFoundErr("LogResult");
            }
            foreach(AddingLogResultDetailCommand item in command.ListErrors){
                var error = await logResultDetailRepository.GetAsync(ww => ww.PartErrorID == item.PartID && ww.LogReportResultID == logResult.ID);
                if (error is null){
                    response.AddNotFoundErr("LogResultDetail");
                }
                error.Detail = item.Detail;
                error.Resolve = item.Resolve;
                error.Status = item.Status;
                await logResultDetailRepository.UpdateAsync(error);
            }
            logResult.ExecutionCompanyID = command.ExecutionCompanyID;
            logResult.DetailedAnalysis = command.DetailedAnalysis;
            logResult.AnalystUUID = CurrentUser.UUID;
            logResult.Suggest = command.Suggest;
            logResult.Conclusion = command.Conclusion;
            logResult.ImageUrls = new List<String> { };

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
        [HttpGet("ResultForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResultForEmail([FromBody] AddingGetLogForEmailCommand command,
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
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logReportResult is null)
            {
                response.AddNotFoundErr("LogReport");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(logReportResult));
            return response.ToIActionResult();
        }
        [HttpPost("ResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateResultFromEmail([FromForm] AddingLogResultFromMailCommand command,
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository,
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogResultDetailRepository logResultDetailRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var result = await logReportResultRepository.CreateAsync(new LogReportResult() 
            {
                AnalystUUID = token.UserUUID,
                Conclusion = command.Conclusion,
                DetailedAnalysis = command.DetailedAnalysis,
                ExecutionCompanyID = command.ExecutionCompanyID,
                ImageUrls = new List<String>{},
                LogFileID = token.LogFileID,
                Suggest = command.Suggest,
            });
            foreach(AddingLogResultDetailCommand item in command.ListErrors){
                var error = new LogResultDetail{
                    Detail = item.Detail,
                    LogReportResultID = result.ID,
                    PartErrorID = item.PartID,
                    Resolve = item.Resolve,
                    Status = item.Status,
                };
                await logResultDetailRepository.CreateAsync(error);
            }
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.Token == command.Token);
            await tokenRepository.DeleteRangeAsync(listLogToken);

            response.SetCreatedObject(result);
            return response.ToIActionResult();
        }
        [HttpPatch("ResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateReportResultFromEmail([FromForm] AddingLogResultFromMailCommand command, [FromServices] LogReportResultRepository logReportResultRepository, 
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository, [FromServices] LogResultDetailRepository logResultDetailRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logResult is null){
                response.AddNotFoundErr("LogResult");
            }
            foreach(AddingLogResultDetailCommand item in command.ListErrors){
                var error = await logResultDetailRepository.GetAsync(ww => ww.PartErrorID == item.PartID && ww.LogReportResultID == logResult.ID);
                if (error is null){
                    response.AddNotFoundErr("LogResultDetail");
                }
                error.Detail = item.Detail;
                error.Resolve = item.Resolve;
                error.Status = item.Status;
                await logResultDetailRepository.UpdateAsync(error);
            }
            logResult.ExecutionCompanyID = command.ExecutionCompanyID;
            logResult.DetailedAnalysis = command.DetailedAnalysis;
            logResult.AnalystUUID = CurrentUser.UUID;
            logResult.Suggest = command.Suggest;
            logResult.Conclusion = command.Conclusion;
            logResult.ImageUrls = new List<String> {};

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpGet("ReportForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportForEmail( 
        [FromBody] AddingGetLogForEmailCommand command, [FromServices] LogReportRepository logReportRepository, 
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                response.AddExpiredErr("Token");
            }
            var log = await logReportRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReport, LogReportViewModel>(log));
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
            var report = new LogReport{
                LogFileID = token.LogFileID,
                AccidentTime = command.AccidentTime, 
                ImageUrls = new List<String>{ },
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                Suggest = command.Suggest, 
                UserUUID = token.UserUUID, 
                PilotName = command.PilotName,
                PartnerCompanyName = command.PartnerCompanyName,
            };
            await logReportRepository.CreateAsync(report);
            
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.Token == command.Token);
            await tokenRepository.DeleteRangeAsync(listLogToken);

            response.SetCreatedObject(report);
            return response.ToIActionResult();
        }
        [HttpPatch("ReportFromEmail")]
        public async Task<IActionResult> UpdateReportFromEmail( 
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
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logReport is null){
                response.AddNotFoundErr("LogReport");
            }
            logReport.UpdatedTime = DateTime.Now;
            logReport.AccidentTime = command.AccidentTime;
            logReport.Suggest = command.Suggest;
            logReport.PilotDescription = command.PilotDescription;
            logReport.ReporterDescription = command.ReporterDescription;
            logReport.ImageUrls = new List<String>{};
            logReport.PilotName = command.PilotName;
            logReport.PartnerCompanyName = command.PartnerCompanyName;

            await logReportRepository.UpdateAsync(logReport);
            return response.ToIActionResult();
        }
        [HttpGet("DetailForEmail")]
        public async Task<IActionResult> GetLogDetailForEmail([FromServices] LogDetailRepository logDetailRepository,
        [FromServices] LogTokenRepository tokenRepository, [FromBody] AddingGetLogForEmailCommand command){
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
        [HttpPost("Errors")]
        public async Task<IActionResult> SendEmailErrors([FromBody] AddingLogErrorCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if(!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == command.Id);
            if (logFile is null){
                response.AddNotFoundErr("LogFile");
            }
            var errorString = String.Empty;
            var contentString = String.Empty;
            if (command.Error == "log") {
                errorString = "File Log bị lỗi";
                contentString = "Yêu cầu phòng IT kiểm tra hệ thống DroneHub và gửi thông tin cho phòng DC sớm nhất có thể";
            }
            else {
                errorString = "Báo cáo có mâu thuẫn";
                contentString = "Phòng DroneControl sẽ mở luồng mail trao dổi kỹ hơn trong vòng 1 ngày";
            }
            foreach(var item in listEmailForError){
                String token = TokenHelper.GenerateToken();
                await emailService.SendMailAsync(new String[] { item }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})", 
                $"Dear,\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\nMã hiệu Drone: {logFile.Device.Name}\nTình trạng: {errorString}\n{contentString}");

            }
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadReportImage")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> UploadReportImage([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }

            for(var i = 0; i < command.Files.Count; i++){
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{id}" });
                report.ImageUrls.Add(fileLink);
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadResultImage")]
        public async Task<IActionResult> UploadResultImage([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if(!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3){
                actionResponse.AddNotAllowedErr();
            }
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }

            for(var i = 0; i < command.Files.Count; i++){
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-results", $"{id}" });
                report.ImageUrls.Add(fileLink);
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadReportImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadReportImageFromEmail([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var token = await logTokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                actionResponse.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                actionResponse.AddExpiredErr("Token");
            }
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }

            for(var i = 0; i < command.Files.Count; i++){
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{id}" });
                report.ImageUrls.Add(fileLink);
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadResultImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadResultImageFromEmail([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var token = await logTokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (token is null){
                actionResponse.AddNotFoundErr("Token");
            }
            if ((DateTime.Now - token.CreateTime).TotalMinutes > 30){
                actionResponse.AddExpiredErr("Token");
            }
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }

            for(var i = 0; i < command.Files.Count; i++){
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{id}" });
                report.ImageUrls.Add(fileLink);
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
    }
}