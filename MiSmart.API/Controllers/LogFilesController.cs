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
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;

using System.Text;
using iText.Html2pdf;

namespace MiSmart.API.Controllers
{
    public class LogFilesController : AuthorizedAPIControllerBase
    {
        public LogFilesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromQuery] Int32? deviceID, [FromServices] LogFileRepository logFileRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromQuery] Int32? PartErrorID,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Boolean isUnstable = false,
        [FromQuery] String relation = "Maintainer")
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            Expression<Func<LogFile, Boolean>> query = ww => false;
            if (relation == "Maintainer")
            {
                if (CurrentUser.RoleID != 3)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                            && (ww.FileBytes.Length > 500000)
                            && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true);
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                                && (ww.FileBytes.Length > 500000)
                                && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true)
                                && (ww.Status == LogStatus.Completed || ww.Status == LogStatus.Approved);
            } 
            else if (relation == "LogAnalyst")
            {
                if (CurrentUser.RoleID != 4)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => ((deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                                && (ww.FileBytes.Length > 500000)
                                && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true)
                                && (ww.Status == LogStatus.Warning || ww.Status == LogStatus.SecondWarning || ww.Status == LogStatus.Completed || ww.Status == LogStatus.Approved)
                                && (PartErrorID.HasValue ? ww.LogReportResult.LogResultDetails.Any(ww => ww.PartErrorID == PartErrorID.Value) : true)
                                && (from.HasValue ? (ww.LoggingTime >= from.Value) : true)
                                && (to.HasValue ? (ww.LoggingTime <= to.Value) : true));
            }

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
        [FromServices] LogFileRepository logFileRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3 && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
            }
            if (!logFile.isAnalyzed)
            {
                actionResponse.AddNotFoundErr("LogDetail");
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == id);
            if (logDetail is null)
                actionResponse.AddNotFoundErr("Detail");

            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogDetail, LogDetailViewModel>(logDetail));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/Report")]
        public async Task<IActionResult> GetLogReport([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3 && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                actionResponse.AddNotFoundErr("Report");
            }
            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogReport, LogReportViewModel>(logReport));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/Result")]
        public async Task<IActionResult> GetLogResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3 && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReportResult is null)
            {
                actionResponse.AddNotFoundErr("ReportResult");
            }
            actionResponse.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(logReportResult));
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/Report")]
        public async Task<IActionResult> CreateReport([FromRoute] Guid id,
        [FromBody] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository,
        [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] LogTokenRepository logTokenRepository,
        [FromServices] IOptions<TargetEmailSettings> options)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }
            if (logFile.Status != LogStatus.Normal)
            {
                response.AddInvalidErr("LogStatus");
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is not null)
            {
                response.AddExistedErr("LogReport");
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
            if (command.Username?.Length != 0)
            {
                report.Username = command.Username;
            }

            await logReportRepository.CreateAsync(report);

            logFile.Status = LogStatus.Warning;
            logFile.DroneStatus = DroneStatus.Fall;
            await logFileRepository.UpdateAsync(logFile);

            foreach (UserEmail item in settings.LogReport)
            {
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken { Token = token, UserUUID = new Guid(item.UUID), LogFileID = id, Username = item.Email });
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/log-report-result?token={token} \n\nThank you");
            }
            response.SetCreatedObject(report);
            return response.ToIActionResult();
        }
        [HttpPatch("{id:Guid}/Report")]
        public async Task<IActionResult> UpdateReport([FromRoute] Guid id, [FromBody] AddingLogReportCommand command, [FromServices] LogReportRepository logReportRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                response.AddNotFoundErr("LogReport");
            }
            logReport.UpdatedTime = DateTime.UtcNow;
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
        [FromServices] PartRepository partRepository,
        [FromServices] LogResultDetailRepository logResultDetailRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }
            if (logFile.Status != LogStatus.Warning && logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == logFile.ID);
            if (logResult is not null)
                response.AddExistedErr("ResultReport");
            var result = new LogReportResult
            {
                ImageUrls = new List<String> { },
                DetailedAnalysis = command.DetailedAnalysis,
                LogFileID = id,
                AnalystUUID = CurrentUser.UUID,
                Suggest = command.Suggest,
                Conclusion = command.Conclusion,
                ResponsibleCompany = command.ResponsibleCompany,
                AnalystName = CurrentUser.Email
            };
            if (command.ResponsibleCompany == ResponsibleCompany.MiSmart)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => String.Equals(ww.Name, "Công ty khai thác MiSmart"));
                result.ExecutionCompany = executionCompany;
            }
            else if (command.ResponsibleCompany == ResponsibleCompany.AnotherCompany)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == logFile.Device.ExecutionCompanyID.GetValueOrDefault());
                if (executionCompany is not null)
                {
                    result.ExecutionCompany = executionCompany;
                }
            }
            var res = await logReportResultRepository.CreateAsync(result);

            foreach (AddingLogResultDetailCommand item in command.ListErrors)
            {
                var part = await partRepository.GetAsync(ww => ww.ID == item.PartID.GetValueOrDefault());
                if (part is null)
                {
                    continue;
                }
                var error = new LogResultDetail
                {
                    Detail = item.Detail,
                    LogReportResultID = res.ID,
                    PartErrorID = item.PartID.GetValueOrDefault(),
                    Resolve = item.Resolve,
                    Status = item.Status,
                };
                await logResultDetailRepository.CreateAsync(error);
            }

            logFile.Status = LogStatus.Completed;
            await logFileRepository.UpdateAsync(logFile);

            response.SetCreatedObject(result);
            return response.ToIActionResult();
        }

        [HttpPatch("{id:Guid}/Result")]
        public async Task<IActionResult> UpdateReportResult([FromRoute] Guid id, [FromBody] AddingLogResultCommand command,
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MinioService minioService,
        [FromServices] LogResultDetailRepository logResultDetailRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                response.AddNotAllowedErr();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
            }
            foreach (AddingLogResultDetailCommand item in command.ListErrors)
            {
                var error = await logResultDetailRepository.GetAsync(ww => ww.PartErrorID == item.PartID.GetValueOrDefault() && ww.LogReportResultID == logResult.ID);
                if (error is null)
                {
                    response.AddNotFoundErr("LogResultDetail");
                }
                error.Detail = item.Detail;
                error.Resolve = item.Resolve;
                error.Status = item.Status;
                await logResultDetailRepository.UpdateAsync(error);
            }
            if (command.ResponsibleCompany == ResponsibleCompany.MiSmart)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => String.Equals(ww.Name, "Công ty khai thác MiSmart"));
                logResult.ExecutionCompany = executionCompany;
            }
            else if (command.ResponsibleCompany == ResponsibleCompany.AnotherCompany)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == logResult.LogFile.Device.ExecutionCompanyID);
                if (executionCompany is not null)
                    logResult.ExecutionCompany = executionCompany;
            }
            logResult.DetailedAnalysis = command.DetailedAnalysis;
            logResult.AnalystUUID = CurrentUser.UUID;
            logResult.AnalystName = CurrentUser.Email;
            logResult.Suggest = command.Suggest;
            logResult.Conclusion = command.Conclusion;
            logResult.ImageUrls = new List<String> { };

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/ApprovedResult")]
        public async Task<IActionResult> ApprovedResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] IOptions<TargetEmailSettings> options, [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] LogFileRepository logFileRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator)
            {
                response.AddNotAllowedErr();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
            }
            logResult.ApproverUUID = CurrentUser.UUID;
            logResult.ApproverName = CurrentUser.Email;
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);

            foreach (UserEmail item in settings.LogReport)
            {
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = token, UserUUID = new Guid(item.UUID), LogFileID = id });
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nKết luận chung: {logResult.Conclusion}\n\nThank you");
            }

            logFile.Status = LogStatus.Approved;
            await logFileRepository.UpdateAsync(logFile);

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpGet("ResultForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResultForEmail([FromQuery] String token,
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository,
        [FromServices] LogReportResultRepository logReportResultRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (logReportResult is null)
            {
                response.AddNotFoundErr("ReportResult");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(logReportResult));
            return response.ToIActionResult();
        }
        [HttpPost("ResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateResultFromEmail([FromBody] AddingLogResultFromMailCommand command,
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] PartRepository partRepository,
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogResultDetailRepository logResultDetailRepository,
        [FromServices] LogFileRepository logFileRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var token = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }
            if (logFile.Status != LogStatus.Warning && logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
            }

            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == logFile.ID);
            if (logResult is not null)
            {
                response.AddExistedErr("ResultReport");
            }
            var result = new LogReportResult()
            {
                AnalystUUID = token.UserUUID,
                Conclusion = command.Conclusion,
                DetailedAnalysis = command.DetailedAnalysis,
                ImageUrls = new List<String> { },
                LogFileID = token.LogFileID,
                Suggest = command.Suggest,
                ResponsibleCompany = command.ResponsibleCompany,
                AnalystName = token.Username,
            };

            if (command.ResponsibleCompany == ResponsibleCompany.MiSmart)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => String.Equals(ww.Name, "Công ty khai thác MiSmart"));
                result.ExecutionCompany = executionCompany;
            }
            else if (command.ResponsibleCompany == ResponsibleCompany.AnotherCompany)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == logFile.Device.ExecutionCompanyID);
                if (executionCompany is not null)
                    result.ExecutionCompany = executionCompany;
            }
            var res = await logReportResultRepository.CreateAsync(result);
            foreach (AddingLogResultDetailCommand item in command.ListErrors)
            {
                var part = await partRepository.GetAsync(ww => ww.ID == item.PartID.GetValueOrDefault());
                if (part is null)
                {
                    continue;
                }
                var error = new LogResultDetail
                {
                    Detail = item.Detail,
                    LogReportResultID = res.ID,
                    PartErrorID = item.PartID.GetValueOrDefault(),
                    Resolve = item.Resolve,
                    Status = item.Status,
                };
                await logResultDetailRepository.CreateAsync(error);
            }
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.LogFileID == token.LogFileID);
            await tokenRepository.DeleteRangeAsync(listLogToken);

            logFile.Status = LogStatus.Completed;
            await logFileRepository.UpdateAsync(logFile);

            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(res));
            return response.ToIActionResult();
        }

        [HttpGet("ReportForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportForEmail(
        [FromQuery] String token, [FromServices] LogReportRepository logReportRepository,
        [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var log = await logReportRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReport, LogReportViewModel>(log));
            return response.ToIActionResult();
        }
        // [HttpPost("ReportFromEmail")]
        // public async Task<IActionResult> CreateReportFromEmail(
        // [FromBody] AddingLogReportFromEmailCommand command, [FromServices] LogReportRepository logReportRepository,
        // [FromServices] MinioService minioService, [FromServices] LogTokenRepository tokenRepository)
        // {
        //     ActionResponse response = actionResponseFactory.CreateInstance();
        //     var token = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
        //     if (token is null)
        //     {
        //         response.AddNotFoundErr("Token");
        //     }
        //     if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
        //     {
        //         response.AddExpiredErr("Token");
        //     }
        //     var report = new LogReport
        //     {
        //         LogFileID = token.LogFileID,
        //         AccidentTime = command.AccidentTime,
        //         ImageUrls = new List<String> { },
        //         PilotDescription = command.PilotDescription,
        //         ReporterDescription = command.ReporterDescription,
        //         Suggest = command.Suggest,
        //         UserUUID = token.UserUUID,
        //         PilotName = command.PilotName,
        //         PartnerCompanyName = command.PartnerCompanyName,
        //     };
        //     await logReportRepository.CreateAsync(report);

        //     var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.Token == command.Token);
        //     await tokenRepository.DeleteRangeAsync(listLogToken);

        //     response.SetCreatedObject(report);
        //     return response.ToIActionResult();
        // }

        [HttpGet("DetailForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLogDetailForEmail([FromServices] LogDetailRepository logDetailRepository,
        [FromServices] LogTokenRepository tokenRepository, [FromQuery] String token)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (logDetail is null)
            {
                response.AddNotFoundErr("LogDetail");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogDetail, LargeLogDetailViewModel>(logDetail));
            return response.ToIActionResult();
        }
        [HttpPost("Errors")]
        public async Task<IActionResult> SendEmailErrors([FromBody] AddingLogErrorCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == command.Id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }
            var errorString = String.Empty;
            var contentString = String.Empty;
            if (command.Error == "log")
            {
                errorString = "File Log bị lỗi";
                contentString = "Yêu cầu phòng IT kiểm tra hệ thống DroneHub và gửi thông tin cho phòng DC sớm nhất có thể";
            }
            else
            {
                errorString = "Báo cáo có mâu thuẫn";
                contentString = "Phòng DroneControl sẽ mở luồng mail trao đổi kỹ hơn trong vòng 1 ngày";
            }

            await emailService.SendMailAsync(settings.LogError.ToArray(), new String[] { }, new String[] { }, @$"Subject: [Dữ liệu bay bị lỗi] Mã hiệu drone ({logFile.Device.Name})",
            $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nTình trạng: {errorString}\n\n{contentString}\n\nThank you");

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

            for (var i = 0; i < command.Files.Count; i++)
            {
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{id}" });
                report.ImageUrls.Add(fileLink);
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadResultImage")]
        public async Task<IActionResult> UploadResultImage([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var result = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (result is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
            }

            for (var i = 0; i < command.Files.Count; i++)
            {
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-results", $"{id}" });
                result.ImageUrls.Add(fileLink);
            }

            await logReportResultRepository.UpdateAsync(result);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("UploadResultImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadResultImageFromEmail([FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            var result = await logReportResultRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (result is null)
            {
                actionResponse.AddNotFoundErr("ResultReport");
            }

            for (var i = 0; i < command.Files.Count; i++)
            {
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-results", $"{result.ID}" });
                result.ImageUrls.Add(fileLink);
            }

            await logReportResultRepository.UpdateAsync(result);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("ApprovedResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ApprovedResultFromEmail([FromBody] AddingGetLogForEmailCommand command, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] IOptions<TargetEmailSettings> options, [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] LogFileRepository logFileRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
            }
            logResult.ApproverUUID = token.UserUUID;
            logResult.ApproverName = token.Username;

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            foreach (UserEmail item in settings.LogReport)
            {
                String generateToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = generateToken, UserUUID = new Guid(item.UUID), LogFileID = token.LogFileID });
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nKết luận chung: {logResult.Conclusion}\n\nThank you");
            }

            logFile.Status = LogStatus.Approved;
            await logFileRepository.UpdateAsync(logFile);

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpPost("ErrorFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmailErrorFromEmail([FromBody] AddingLogErrorForEmailCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }

            var listLogToken = await logTokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.LogFileID == token.LogFileID);
            await logTokenRepository.DeleteRangeAsync(listLogToken);

            var errorString = "Báo cáo có mâu thuẫn";
            var contentString = "Vui lòng kiểm tra và cập nhật lại báo cáo theo đường link sau";
            if (command.Message.Length != 0)
                contentString = "Tin nhắn: " + command.Message + "\n\n" + contentString;
            if (logFile.LogReport.Username is not null && logFile.LogReport.Username?.Length != 0)
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = newToken, UserUUID = logFile.LogReport.UserUUID, LogFileID = logFile.ID, Username = logFile.LogReport.Username });
                await emailService.SendMailAsync(new String[] { logFile.LogReport.Username }, new String[] { }, new String[] { }, @$"Subject: [Báo cáo lỗi] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nTình trạng: {errorString}\n\n{contentString}\n\nLink: https://dronehub.mismart.ai/second-log-report?token={newToken}\n\nThank you");
            }

            logFile.Status = LogStatus.SecondWarning;
            await logFileRepository.UpdateAsync(logFile);

            return response.ToIActionResult();
        }
        [HttpPost("SecondReport")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSecondReport([FromBody] AddingLogReportFromEmailCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options,
        [FromServices] LogTokenRepository logTokenRepository, [FromServices] SecondLogReportRepository secondLogReportRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
            }
            if (logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
            }
            var report = new SecondLogReport
            {
                LogFileID = token.LogFileID,
                AccidentTime = command.AccidentTime,
                ImageUrls = new List<String> { },
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                Suggest = command.Suggest,
                UserUUID = token.UserUUID,
                PilotName = command.PilotName,
                PartnerCompanyName = command.PartnerCompanyName,
                Username = command.Username,
            };
            var secondReport = await secondLogReportRepository.CreateAsync(report);

            var logToken = await logTokenRepository.GetAsync(ww => ww.Token == command.Token);
            await logTokenRepository.DeleteAsync(logToken);

            response.SetData(ViewModelHelpers.ConvertToViewModel<SecondLogReport, SecondLogReportViewModel>(secondReport));

            foreach (UserEmail item in settings.LogReport)
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken { Token = newToken, UserUUID = new Guid(item.UUID), LogFileID = logFile.ID, Username = item.Email });
                await emailService.SendMailAsync(new String[] { item.Email }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích lần 2] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/second-log-result?token={newToken} \n\nThank you");
            }

            return response.ToIActionResult();
        }
        [HttpPost("UploadSecondReportImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadReportImageFromEmail([FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] MinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            var secondReport = await secondLogReportRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (secondReport is null)
            {
                actionResponse.AddNotFoundErr("SecondReport");
            }

            for (var i = 0; i < command.Files.Count; i++)
            {
                var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{secondReport.ID}" });
                secondReport.ImageUrls.Add(fileLink);
            }

            await secondLogReportRepository.UpdateAsync(secondReport);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpGet("GetSecondReportFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSecondReportFromEmail([FromQuery] String token, [FromServices] SecondLogReportRepository secondLogReportRepository,
        [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
            }
            var secondReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (secondReport is null)
                actionResponse.AddNotFoundErr("SecondReport");
            actionResponse.SetData((ViewModelHelpers.ConvertToViewModel<SecondLogReport, SecondLogReportViewModel>(secondReport)));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/SecondReport")]
        public async Task<IActionResult> GetSecondReport([FromRoute] Guid id, [FromServices] SecondLogReportRepository secondLogReportRepository,
        [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3 && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var secondReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (secondReport is null)
                actionResponse.AddNotFoundErr("SecondReport");
            actionResponse.SetData((ViewModelHelpers.ConvertToViewModel<SecondLogReport, SecondLogReportViewModel>(secondReport)));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("GetFileForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFileForEmail([FromServices] LogFileRepository logFileRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromQuery] String token, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
            }
            Expression<Func<LogFile, Boolean>> query = ww => (ww.ID == resToken.LogFileID);
            var logFile = await logFileRepository.GetAsync(query);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
            }

            actionResponse.SetFile(logFile.FileBytes, "application/octet-stream", logFile.FileName);
            return actionResponse.ToIActionResult();
        }
        [HttpGet("UsernameFromToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsernameFromToken([FromQuery] String token, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
            }
            actionResponse.SetData(new { name = resToken.Username });
            return actionResponse.ToIActionResult();
        }

        [HttpGet("DownloadResultReport")]
        [AllowAnonymous]
        public async Task<IActionResult>  DownloadResultReport([FromQuery] Guid id, [FromServices] LogReportRepository logReportRepository,
        [FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] LogFileRepository logFileRepository, [FromServices] MyEmailService emailService)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var name = "ResultWithOneReport";
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
                actionResponse.AddNotFoundErr("LogReport");
            var secondLogReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (secondLogReport is not null)
                name = "ResultWithTwoReport";
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
                actionResponse.AddNotFoundErr("ResultReport");
            var html = emailService.GetHTML(name);
            StringBuilder htmlString = new StringBuilder(html);
            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (name == "ResultWithOneReport")
            {
                htmlString.Replace("flight_id", "Chưa định danh");
                htmlString.Replace("time", TimeZoneInfo.ConvertTimeFromUtc(logResult.UpdatedTime, seaTimeZone).ToString());
                htmlString.Replace("name", logReport.Username);
                htmlString.Replace("drone_id", logReport.LogFile.Device.Name);
                htmlString.Replace("location", logReport.LogFile.LogDetail?.Location);
                htmlString.Replace("accident_time", TimeZoneInfo.ConvertTimeFromUtc(logReport.AccidentTime, seaTimeZone).ToString());
                htmlString.Replace("pilot", logReport.PilotName);
                htmlString.Replace("partner_company", logReport.LogFile.Device.ExecutionCompany?.Name);
                htmlString.Replace("pilot_description", logReport.PilotDescription);
                htmlString.Replace("reporter_description", logReport.ReporterDescription);
                var listImageReport = "";
                foreach(var ImageUrl in logReport.ImageUrls){
                    var item = emailService.GetHTML("ImageItem");
                    item = item.Replace("img_src", ImageUrl);
                    listImageReport += item;
                }
                htmlString.Replace("list_image_report", listImageReport);
                var listError = logResult.LogResultDetails.ToArray();
                var tableData = "";
                for (int i = 0; i < listError.Count(); i++)
                {
                    var error = listError[i];
                    var row = emailService.GetHTML("ResultDetailRow");
                    row = row.Replace("stt", (i + 1).ToString());
                    row = row.Replace("group", error.PartError.Group);
                    row = row.Replace("name_error", error.PartError.Name);
                    if (error.Status == StatusError.Good)
                        row = row.Replace("status_good", "checked");
                    else if (error.Status == StatusError.Bad)
                        row = row.Replace("status_bad", "checked");
                    else
                        row = row.Replace("status_follow", "checked");
                    row = row.Replace("error_detail", error.Detail);
                    row = row.Replace("measure", error.Resolve);
                    tableData += row;
                }
                htmlString.Replace("table_data_indicator", tableData);
                htmlString.Replace("responsible_company", logResult.ResponsibleCompany.ToString());
                htmlString.Replace("conclusion", logResult.Conclusion);
                htmlString.Replace("detailed_analysis", logResult.DetailedAnalysis);
                htmlString.Replace("result_suggestion", logResult.Suggest);
                htmlString.Replace("analyst", logResult.AnalystName);
                htmlString.Replace("approver", logResult.ApproverName);
                var listImageResult = "";
                foreach(var ImageUrl in logResult.ImageUrls){
                    var item = emailService.GetHTML("ImageItem");
                    item = item.Replace("img_src", ImageUrl);
                    listImageResult += item;
                }
                htmlString.Replace("list_image_result", listImageResult);
            }
            else
            {
                htmlString.Replace("flight_id", "Chưa định danh");
                htmlString.Replace("time", TimeZoneInfo.ConvertTimeFromUtc(logResult.UpdatedTime, seaTimeZone).ToString());
                htmlString.Replace("name_1", logReport.Username);
                htmlString.Replace("drone_id_1", logReport.LogFile.Device.Name);
                htmlString.Replace("location_1", logReport.LogFile.LogDetail?.Location);
                htmlString.Replace("accident_time_1", TimeZoneInfo.ConvertTimeFromUtc(logReport.AccidentTime, seaTimeZone).ToString());
                htmlString.Replace("pilot_1", logReport.PilotName);
                htmlString.Replace("partner_company_1", logReport.LogFile.Device.ExecutionCompany?.Name);
                htmlString.Replace("pilot_description_1", logReport.PilotDescription);
                htmlString.Replace("reporter_description_1", logReport.ReporterDescription);
                
                var listImageReport1 = "";
                foreach(var ImageUrl in logReport.ImageUrls){
                    var item = emailService.GetHTML("ImageItem");
                    item = item.Replace("img_src", ImageUrl);
                    listImageReport1 += item;
                }
                htmlString.Replace("list_image_report_1", listImageReport1);
                
                htmlString.Replace("name_2", secondLogReport.Username);
                htmlString.Replace("drone_id_2", secondLogReport.LogFile.Device.Name);
                htmlString.Replace("location_2", secondLogReport.LogFile.LogDetail?.Location);
                htmlString.Replace("accident_time_2", TimeZoneInfo.ConvertTimeFromUtc(secondLogReport.AccidentTime, seaTimeZone).ToString());
                htmlString.Replace("pilot_2", secondLogReport.PilotName);
                htmlString.Replace("partner_company_2", secondLogReport.LogFile.Device.ExecutionCompany?.Name);
                htmlString.Replace("pilot_description_2", secondLogReport.PilotDescription);
                htmlString.Replace("reporter_description_2", secondLogReport.ReporterDescription);

                var listImageReport2 = "";
                foreach(var ImageUrl in secondLogReport.ImageUrls){
                    var item = emailService.GetHTML("ImageItem");
                    item = item.Replace("img_src", ImageUrl);
                    listImageReport2 += item;
                }
                htmlString.Replace("list_image_report_2", listImageReport2);

                var listError = logResult.LogResultDetails.ToArray();
                var tableData = "";
                for (int i = 0; i < listError.Count(); i++)
                {
                    var error = listError[i];
                    var row = emailService.GetHTML("ResultDetailRow");
                    row = row.Replace("stt", (i + 1).ToString());
                    row = row.Replace("name_error", error.PartError.Name);
                    if (error.Status == StatusError.Good)
                        row = row.Replace("status_good", "checked");
                    else if (error.Status == StatusError.Bad)
                        row = row.Replace("status_bad", "checked");
                    else
                        row = row.Replace("status_follow", "checked");
                    row = row.Replace("error_detail", error.PartError.Name);
                    row = row.Replace("measure", error.Resolve);
                    tableData += row;
                }
                htmlString.Replace("table_data_indicator", tableData);
                htmlString.Replace("responsible_company", logResult.ResponsibleCompany.ToString());
                htmlString.Replace("conclusion", logResult.Conclusion);
                htmlString.Replace("detailed_analysis", logResult.DetailedAnalysis);
                htmlString.Replace("result_suggestion", logResult.Suggest);
                htmlString.Replace("analyst", logResult.AnalystName);
                htmlString.Replace("approver", logResult.ApproverName);
            }
            Byte[] bytes = null;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MemoryStream ms = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(htmlString.ToString(), ms);
                bytes = ms.ToArray();
            }
            actionResponse.SetFile(bytes, "application/pdf", "report.pdf");
            return actionResponse.ToIActionResult();
        }
    }
}