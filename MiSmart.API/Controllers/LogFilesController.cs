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
using System.Collections.Generic;
using MiSmart.API.Services;
using MiSmart.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;

using System.Text;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;

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
        [FromQuery] String? flightID,
        [FromQuery] Boolean isUnstable = false,
        [FromQuery] String? relation = "Maintainer")
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
                            && (ww.FileBytes != null ? ww.FileBytes.Length > 500000 : false)
                            && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true)
                                && (PartErrorID.HasValue ? ((ww.LogReportResult != null && ww.LogReportResult.LogResultDetails != null) ? ww.LogReportResult.LogResultDetails.Any(ww => ww.PartErrorID == PartErrorID.Value && ww.Status == StatusError.Bad) : false) : true)
                                && (from.HasValue ? (ww.LoggingTime >= from.Value) : true)
                                && (to.HasValue ? (ww.LoggingTime <= to.Value) : true)
                                && (!String.IsNullOrWhiteSpace(flightID) ? (ww.FlightID.ToString().Contains(flightID)) : true);
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                                && (ww.FileBytes != null ? ww.FileBytes.Length > 500000 : false)
                                && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true)
                                    && (PartErrorID.HasValue ? ((ww.LogReportResult != null && ww.LogReportResult.LogResultDetails != null) ? ww.LogReportResult.LogResultDetails.Any(ww => ww.PartErrorID == PartErrorID.Value && ww.Status == StatusError.Bad) : false) : true)
                                    && (from.HasValue ? (ww.LoggingTime >= from.Value) : true)
                                    && (to.HasValue ? (ww.LoggingTime <= to.Value) : true)
                                    && (!String.IsNullOrWhiteSpace(flightID) ? (ww.FlightID.ToString().Contains(flightID)) : true);
            }
            else if (relation == "LogAnalyst")
            {
                if (CurrentUser.RoleID != 4)
                {
                    actionResponse.AddNotAllowedErr();
                }
                query = ww => (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                                && (ww.FileBytes != null ? ww.FileBytes.Length > 500000 : false)
                                && (isUnstable == true ? (ww.DroneStatus != DroneStatus.Stable) : true)
                                && (ww.Status == LogStatus.Warning || ww.Status == LogStatus.SecondWarning || ww.Status == LogStatus.Completed || ww.Status == LogStatus.Approved)
                                    && (PartErrorID.HasValue ? ((ww.LogReportResult != null && ww.LogReportResult.LogResultDetails != null) ? ww.LogReportResult.LogResultDetails.Any(ww => ww.PartErrorID == PartErrorID.Value && ww.Status == StatusError.Bad) : false) : true)
                                    && (from.HasValue ? (ww.LoggingTime >= from.Value) : true)
                                    && (to.HasValue ? (ww.LoggingTime <= to.Value) : true)
                                    && (!String.IsNullOrWhiteSpace(flightID) ? (ww.FlightID.ToString().Contains(flightID)) : true);
            }

            var listResponse = await logFileRepository.GetListResponseViewAsync<LogFileViewModel>(pageCommand, query, ww => ww.LoggingTime, false);

            listResponse.SetResponse(actionResponse);
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:Guid}/File")]

        public async Task<IActionResult> GetFile([FromRoute] Guid id, [FromServices] LogFileRepository logFileRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (CurrentUser.RoleID != 3 && CurrentUser.RoleID != 4 && CurrentUser.RoleID != 1 && !CurrentUser.IsAdministrator)
            {
                actionResponse.AddNotAllowedErr();
                return actionResponse.ToIActionResult();
            }
            Expression<Func<LogFile, Boolean>> query = ww => (ww.ID == id);
            var logFile = await logFileRepository.GetAsync(query);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
                return actionResponse.ToIActionResult();
            }

            actionResponse.SetFile(logFile.FileBytes ?? new Byte[0], "application/octet-stream", logFile.FileName ?? "ex.bin");
            return actionResponse.ToIActionResult();
        }
        [HttpGet("GetZipFile")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> GetZipFile([FromServices] LogFileRepository logFileRepository, [FromQuery] PageCommand pageCommand, [FromQuery] Int32? deviceID, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            Expression<Func<LogFile, Boolean>> query = ww => (ww.FileBytes != null ? ww.FileBytes.Length >= 500000 : false) && (deviceID.HasValue ? ww.DeviceID == deviceID.GetValueOrDefault() : true) && (from.HasValue ? ww.LoggingTime >= from.GetValueOrDefault() : true) && (to.HasValue ? ww.LoggingTime <= to.GetValueOrDefault() : true);
            var data = await logFileRepository.GetListEntitiesAsync(pageCommand, query);
            if (data.Count == 0)
            {
                actionResponse.AddNotFoundErr("ZipFile");
                return actionResponse.ToIActionResult();
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
                            var path = $"{item.Key?.Name ?? ""}_{item.Key?.ID ?? 0}/{fileItem.FileName}";
                            var fileEntry = archive.CreateEntry(path);

                            using (var entryStream = fileEntry.Open())
                            {
                                var fileBytes = fileItem.FileBytes;
                                if (fileBytes != null)
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
            LogFile? logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
                return actionResponse.ToIActionResult();
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == id);
            if (logDetail is null)
            {
                actionResponse.AddNotFoundErr("Detail");
                return actionResponse.ToIActionResult();
            }
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
                return actionResponse.ToIActionResult();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                actionResponse.AddNotFoundErr("Report");
                return actionResponse.ToIActionResult();
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
                return actionResponse.ToIActionResult();
            }
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReportResult is null)
            {
                actionResponse.AddNotFoundErr("ReportResult");
                return actionResponse.ToIActionResult();
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
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }
            if (logFile.Status != LogStatus.Normal)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is not null)
            {
                response.AddExistedErr("LogReport");
                return response.ToIActionResult();
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
                Username = CurrentUser.Email,
            };

            await logReportRepository.CreateAsync(report);

            logFile.Status = LogStatus.Warning;
            logFile.DroneStatus = DroneStatus.Fall;
            await logFileRepository.UpdateAsync(logFile);

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            foreach (UserEmail item in settings.LogReport ?? new List<UserEmail>())
            {
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken { Token = token, UserUUID = item.UUID != null ? new Guid(item.UUID) : Guid.Empty, LogFileID = id, Username = item.Email });
                await emailService.SendMailAsync(new String[] { item.Email ?? "" }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/log-report-result?token={token} \n\nThank you");
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
                return response.ToIActionResult();
            }
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                response.AddNotFoundErr("LogReport");
                return response.ToIActionResult();
            }
            if (logReport.LogFile is not null && logReport.LogFile.Status != LogStatus.Warning)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
            }
            logReport.UpdatedTime = DateTime.UtcNow;
            logReport.AccidentTime = command.AccidentTime;
            logReport.Suggest = command.Suggest;
            logReport.PartnerCompanyName = command.PartnerCompanyName;
            logReport.PilotDescription = command.PilotDescription;
            logReport.ReporterDescription = command.ReporterDescription;
            logReport.UserUUID = CurrentUser.UUID;
            logReport.Username = CurrentUser.Email;
            logReport.PilotName = command.PilotName;
            await logReportRepository.UpdateAsync(logReport);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/Result")]
        public async Task<IActionResult> CreateReportResult([FromRoute] Guid id, [FromBody] AddingLogResultCommand command,
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogFileRepository logFileRepository,
        [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] PartRepository partRepository,
        [FromServices] LogResultDetailRepository logResultDetailRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromServices] IOptions<TargetEmailSettings> options)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }
            if (logFile.Status != LogStatus.Warning && logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
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
                AnalystName = CurrentUser.Email,
            };
            if (command.ResponsibleCompany == ResponsibleCompany.MiSmart)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => String.Equals(ww.Name, "Công ty khai thác MiSmart"));
                result.ExecutionCompany = executionCompany;
            }
            else if (command.ResponsibleCompany == ResponsibleCompany.AnotherCompany)
            {
                if (logFile.Device is not null)
                {
                    var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == logFile.Device.ExecutionCompanyID.GetValueOrDefault());
                    if (executionCompany is not null)
                    {
                        result.ExecutionCompany = executionCompany;
                    }
                }
            }
            else
            {
                result.ExecutionCompany = null;
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

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (logFile.Device is not null)
                await emailService.SendMailAsync(settings.ApprovedLogReport?.ToArray() ?? new String[0], new String[] { }, new String[] { }, @$"[Báo cáo cần xác nhận] Mã hiệu drone ({logFile.Device.Name})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device.Name}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nVui lòng vào trang Drone Hub để xác nhận báo cáo\n\nThank you");

            logFile.Status = LogStatus.Completed;
            await logFileRepository.UpdateAsync(logFile);

            response.SetCreatedObject(result);
            return response.ToIActionResult();
        }

        [HttpPatch("{id:Guid}/Result")]
        public async Task<IActionResult> UpdateReportResult([FromRoute] Guid id, [FromBody] AddingLogResultCommand command,
        [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] LogResultDetailRepository logResultDetailRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
                return response.ToIActionResult();
            }
            if (logResult.LogFile != null && logResult.LogFile.Status != LogStatus.Completed)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
            }
            foreach (AddingLogResultDetailCommand item in command.ListErrors)
            {
                var error = await logResultDetailRepository.GetAsync(ww => ww.PartErrorID == item.PartID.GetValueOrDefault() && ww.LogReportResultID == logResult.ID);
                if (error is null)
                {
                    response.AddNotFoundErr("LogResultDetail");
                    return response.ToIActionResult();
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

                var executionCompany = await executionCompanyRepository.GetAsync(ww => (logResult.LogFile != null && logResult.LogFile.Device != null) ? ww.ID == logResult.LogFile.Device.ExecutionCompanyID : false);
                if (executionCompany is not null)
                    logResult.ExecutionCompany = executionCompany;

            }
            else
            {
                logResult.ExecutionCompany = null;
            }

            logResult.ResponsibleCompany = command.ResponsibleCompany;
            logResult.DetailedAnalysis = command.DetailedAnalysis;
            logResult.AnalystUUID = CurrentUser.UUID;
            logResult.AnalystName = CurrentUser.Email;
            logResult.Suggest = command.Suggest;
            logResult.Conclusion = command.Conclusion;
            logResult.UpdatedTime = DateTime.UtcNow;

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (logResult.LogFile is not null && logResult.LogFile.Device is not null)
            {
                await emailService.SendMailAsync(settings.ApprovedLogReport?.ToArray() ?? new String[0], new String[] { }, new String[] { }, @$"[Báo cáo cần xác nhận] Mã hiệu drone ({logResult.LogFile.Device.Name})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logResult.LogFile.Device.Name}\n\nMã chuyến bay: {logResult.LogFile.FlightID}\n\nThời gian ghi log:  {TimeZoneInfo.ConvertTimeFromUtc(logResult.LogFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nVui lòng vào trang trang Drone Hub để xác nhận báo cáo\n\nThank you");
            }

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/ApprovedResult")]
        public async Task<IActionResult> ApprovedResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] IOptions<TargetEmailSettings> options, [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] LogFileRepository logFileRepository, [FromServices] LogReportRepository logReportRepository, [FromServices] SecondLogReportRepository secondLogReportRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (!CurrentUser.IsAdministrator)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }

            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
                return response.ToIActionResult();
            }

            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                response.AddNotFoundErr("LogReport");
                return response.ToIActionResult();
            }
            logResult.ApproverUUID = CurrentUser.UUID;
            logResult.ApproverName = CurrentUser.Email;

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }

            var secondLogReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);

            StringBuilder html = emailService.GenerateResultLogReport(logResult, logReport, secondLogReport);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MemoryStream ms = new MemoryStream())
            {
                ConverterProperties properties = new ConverterProperties();
                properties.SetFontProvider(new DefaultFontProvider(true, true, true));
                HtmlConverter.ConvertToPdf(html.ToString(), ms);
                Byte[] bytes = ms.ToArray();
                String token = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = token, UserUUID = logReport.UserUUID, LogFileID = id });
                if (logReport.Username != null && logFile.Device != null)
                    await emailService.SendMailAttachmentAsync(new String[] { logReport.Username }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device.Name})",
                    $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã chuyến bay: {logFile.FlightID}\n\nThank you", null, null, bytes, "Kết quả.pdf", false);
            }

            logFile.Status = LogStatus.Approved;
            await logFileRepository.UpdateAsync(logFile);

            await logReportResultRepository.UpdateAsync(logResult);
            return response.ToIActionResult();
        }
        [HttpGet("ResultForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResultForEmail([FromQuery] String? token,
        [FromServices] LogTokenRepository tokenRepository,
        [FromServices] LogReportResultRepository logReportResultRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logReportResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (logReportResult is null)
            {
                response.AddNotFoundErr("ReportResult");
                return response.ToIActionResult();
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(logReportResult));
            return response.ToIActionResult();
        }
        [HttpPost("ResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateResultFromEmail([FromBody] AddingLogResultFromMailCommand command,
        [FromServices] LogTokenRepository tokenRepository,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] PartRepository partRepository,
        [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] LogResultDetailRepository logResultDetailRepository,
        [FromServices] LogFileRepository logFileRepository, [FromServices] IOptions<TargetEmailSettings> options,
        [FromServices] MyEmailService emailService)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();

            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }
            if (logFile.Status != LogStatus.Warning && logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
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
                var executionCompany = await executionCompanyRepository.GetAsync(ww => logFile.Device != null ? ww.ID == logFile.Device.ExecutionCompanyID : false);
                if (executionCompany is not null)
                    result.ExecutionCompany = executionCompany;
            }
            var res = await logReportResultRepository.CreateAsync(result);
            if (command.ListErrors != null)
            {
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
            }
            var listLogToken = await tokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.LogFileID == token.LogFileID);
            await tokenRepository.DeleteRangeAsync(listLogToken);

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            await emailService.SendMailAsync(settings.ApprovedLogReport?.ToArray() ?? new String[0], new String[] { }, new String[] { }, @$"[Báo cáo cần xác nhận] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
            $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nVui lòng vào trang trang Drone Hub để xác nhận báo cáo\n\nThank you");

            logFile.Status = LogStatus.Completed;
            await logFileRepository.UpdateAsync(logFile);

            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReportResult, LogReportResultViewModel>(res));
            return response.ToIActionResult();
        }

        [HttpGet("ReportForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportForEmail(
        [FromQuery] String? token, [FromServices] LogReportRepository logReportRepository,
        [FromServices] LogTokenRepository tokenRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }
            var log = await logReportRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (log is null)
            {
                response.AddNotFoundErr("LogReport");
                return response.ToIActionResult();
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogReport, LogReportViewModel>(log));
            return response.ToIActionResult();
        }

        [HttpGet("DetailForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLogDetailForEmail([FromServices] LogDetailRepository logDetailRepository,
        [FromServices] LogTokenRepository tokenRepository, [FromQuery] String? token)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await tokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }
            var logDetail = await logDetailRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (logDetail is null)
            {
                response.AddNotFoundErr("LogDetail");
                return response.ToIActionResult();
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogDetail, LargeLogDetailViewModel>(logDetail));
            return response.ToIActionResult();
        }

        [HttpGet("LogFileInformationForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLogFileInformationForEmail([FromServices] LogFileRepository logFileRepository, [FromServices] LogTokenRepository logTokenRepository, [FromQuery] String? token)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == resToken.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogDetail");
                return response.ToIActionResult();
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<LogFile, LogFileViewModel>(logFile));
            return response.ToIActionResult();
        }

        [HttpPost("{id:Guid}/SendEmailErrors")]
        public async Task<IActionResult> SendEmailErrors([FromRoute] Guid id, [FromBody] AddingLogErrorCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options,
        [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (CurrentUser.RoleID != 4)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }

            var errorString = "Báo cáo có mâu thuẫn";
            var contentString = "Vui lòng kiểm tra và cập nhật lại báo cáo theo đường link sau";
            if (command.Message != null && command.Message.Length != 0)
                contentString = "Tin nhắn: " + command.Message + "\n\n" + contentString;

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (logFile.LogReport != null && logFile.LogReport.Username != null && logFile.LogReport.Username?.Length != 0)
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = newToken, UserUUID = logFile.LogReport.UserUUID, LogFileID = logFile.ID, Username = logFile.LogReport.Username });
                await emailService.SendMailAsync(new String[] { logFile.LogReport.Username ?? "" }, new String[] { }, new String[] { }, @$"Subject: [Báo cáo lỗi] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nTình trạng: {errorString}\n\n{contentString}\n\nLink: https://dronehub.mismart.ai/second-log-report?token={newToken}\n\nThank you");
            }

            logFile.Status = LogStatus.SecondWarning;
            await logFileRepository.UpdateAsync(logFile);

            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadReportImage")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> UploadReportImage([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository, [FromServices] MyMinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
                return actionResponse.ToIActionResult();
            }
            if (command.Files != null)
            {
                if (report.ImageUrls is null) report.ImageUrls = new List<String>();
                for (var i = 0; i < command.Files.Count; i++)
                {
                    var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-reports", $"{id}" });
                    report.ImageUrls.Add(fileLink);
                }
            }

            await logReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadResultImage")]
        public async Task<IActionResult> UploadResultImage([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MyMinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
                return actionResponse.ToIActionResult();
            }
            var result = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (result is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
                return actionResponse.ToIActionResult();
            }

            if (command.Files != null)
            {
                if (result.ImageUrls is null) result.ImageUrls = new List<String>();
                for (var i = 0; i < command.Files.Count; i++)
                {
                    var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-results", $"{id}" });
                    result.ImageUrls.Add(fileLink);
                }
            }

            await logReportResultRepository.UpdateAsync(result);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("UploadResultImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadResultImageFromEmail([FromServices] LogReportResultRepository logReportResultRepository, [FromServices] MyMinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            var result = await logReportResultRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (result is null)
            {
                actionResponse.AddNotFoundErr("ResultReport");
                return actionResponse.ToIActionResult();
            }
            if (command.Files != null)
            {
                if (result.ImageUrls is null) result.ImageUrls = new List<String>();
                for (var i = 0; i < command.Files.Count; i++)
                {
                    var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "log-results", $"{result.LogFileID}" });
                    result.ImageUrls.Add(fileLink);
                }
            }

            await logReportResultRepository.UpdateAsync(result);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("ApprovedResultFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ApprovedResultFromEmail([FromBody] AddingGetLogForEmailCommand command, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] IOptions<TargetEmailSettings> options, [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyEmailService emailService,
        [FromServices] LogFileRepository logFileRepository, [FromServices] LogReportRepository logReportRepository, [FromServices] SecondLogReportRepository secondLogReportRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logResult is null)
            {
                response.AddNotFoundErr("LogResult");
                return response.ToIActionResult();
            }
            logResult.ApproverUUID = token.UserUUID;
            logResult.ApproverName = token.Username;

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }

            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);
            if (logReport is null)
            {
                response.AddNotFoundErr("LogReport");
                return response.ToIActionResult();
            }

            var secondLogReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == token.LogFileID);

            StringBuilder html = emailService.GenerateResultLogReport(logResult, logReport, secondLogReport);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MemoryStream ms = new MemoryStream())
            {
                ConverterProperties properties = new ConverterProperties();
                properties.SetFontProvider(new DefaultFontProvider(true, true, true));
                HtmlConverter.ConvertToPdf(html.ToString(), ms);
                Byte[] bytes = ms.ToArray();
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = newToken, UserUUID = logReport.UserUUID, LogFileID = logFile.ID });
                if (logReport.Username != null)
                {
                    await emailService.SendMailAttachmentAsync(new String[] { logReport.Username }, new String[] { }, new String[] { }, @$"Subject: [Kết quả Phân tích Dữ liệu bay] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                    $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã chuyến bay: {logFile.FlightID}\n\nThank you", null, null, bytes, "Kết quả", false);
                }
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
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }

            var listLogToken = await logTokenRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.LogFileID == token.LogFileID);
            await logTokenRepository.DeleteRangeAsync(listLogToken);

            var errorString = "Báo cáo có mâu thuẫn";
            var contentString = "Vui lòng kiểm tra và cập nhật lại báo cáo theo đường link sau";
            if (command.Message != null && command.Message.Length != 0)
                contentString = "Tin nhắn: " + command.Message + "\n\n" + contentString;

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (logFile.LogReport != null && logFile.LogReport.Username is not null && logFile.LogReport.Username?.Length != 0)
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken() { Token = newToken, UserUUID = logFile.LogReport.UserUUID, LogFileID = logFile.ID, Username = logFile.LogReport.Username });
                await emailService.SendMailAsync(new String[] { logFile.LogReport.Username ?? "" }, new String[] { }, new String[] { }, @$"Subject: [Báo cáo lỗi] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                $"Dear,\n\nPhòng Điều khiển bay trả Kết quả phân tích Dữ liệu bay:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nTình trạng: {errorString}\n\n{contentString}\n\nLink: https://dronehub.mismart.ai/second-log-report?token={newToken}\n\nThank you");
            }

            logFile.Status = LogStatus.SecondWarning;
            await logFileRepository.UpdateAsync(logFile);

            return response.ToIActionResult();
        }
        [HttpPost("CreateSecondReportFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSecondReportFromEmail([FromBody] AddingSecondLogReportFromEmailCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options,
        [FromServices] LogTokenRepository logTokenRepository, [FromServices] SecondLogReportRepository secondLogReportRepository,
        [FromServices] MyMinioService minioService)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            var token = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (token is null)
            {
                response.AddNotFoundErr("Token");
                return response.ToIActionResult();
            }
            if ((DateTime.UtcNow - token.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                response.AddExpiredErr("Token");
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == token.LogFileID);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }
            if (logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
                return response.ToIActionResult();
            }
            var report = new SecondLogReport
            {
                LogFileID = token.LogFileID,
                AccidentTime = command.AccidentTime,
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                Suggest = command.Suggest,
                UserUUID = token.UserUUID,
                PilotName = command.PilotName,
                PartnerCompanyName = command.PartnerCompanyName,
                Username = command.Username,
                ImageUrls = new List<String>()
            };

            foreach (var url in command.ImageUrls)
            {
                var fileLink = await minioService.CopyObjectAsync(url, new String[] { "drone-hub-api", "second-log-reports", $"{logFile.ID}" });
                report.ImageUrls.Add(fileLink);
            }

            var secondReport = await secondLogReportRepository.CreateAsync(report);

            var logToken = await logTokenRepository.GetAsync(ww => ww.Token == command.Token);
            if (logToken != null)
                await logTokenRepository.DeleteAsync(logToken);

            response.SetData(ViewModelHelpers.ConvertToViewModel<SecondLogReport, SecondLogReportViewModel>(secondReport));

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            foreach (UserEmail item in settings.LogReport ?? new List<UserEmail>())
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken { Token = newToken, UserUUID = item.UUID != null ? new Guid(item.UUID) : Guid.Empty, LogFileID = logFile.ID, Username = item.Email });
                await emailService.SendMailAsync(new String[] { item.Email ?? "" }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích lần 2] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/second-log-result?token={newToken} \n\nThank you");
            }

            return response.ToIActionResult();
        }
        [HttpPost("UploadSecondReportImageFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadReportImageFromEmail([FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] MyMinioService minioService, [FromForm] AddingLogImageLinkFromEmailCommand command, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            var secondReport = await secondLogReportRepository.GetAsync(ww => String.Equals(ww.Token, command.Token));
            if (secondReport is null)
            {
                actionResponse.AddNotFoundErr("SecondReport");
                return actionResponse.ToIActionResult();
            }

            if (command.Files != null)
            {
                if (secondReport.ImageUrls is null) secondReport.ImageUrls = new List<String>();
                for (var i = 0; i < command.Files.Count; i++)
                {
                    var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "second-log-reports", $"{secondReport.LogFileID}" });
                    secondReport.ImageUrls.Add(fileLink);
                }
            }

            await secondLogReportRepository.UpdateAsync(secondReport);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpGet("GetSecondReportFromEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSecondReportFromEmail([FromQuery] String? token, [FromServices] SecondLogReportRepository secondLogReportRepository,
        [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
                return actionResponse.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
                return actionResponse.ToIActionResult();
            }
            var secondReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == resToken.LogFileID);
            if (secondReport is null)
            {
                actionResponse.AddNotFoundErr("SecondReport");
                return actionResponse.ToIActionResult();
            }
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
                return actionResponse.ToIActionResult();
            }
            var secondReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (secondReport is null)
            {
                actionResponse.AddNotFoundErr("SecondReport");
                return actionResponse.ToIActionResult();
            }
            actionResponse.SetData((ViewModelHelpers.ConvertToViewModel<SecondLogReport, SecondLogReportViewModel>(secondReport)));
            return actionResponse.ToIActionResult();
        }
        [HttpGet("GetFileForEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFileForEmail([FromServices] LogFileRepository logFileRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromQuery] String? token, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
                return actionResponse.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
                return actionResponse.ToIActionResult();
            }
            Expression<Func<LogFile, Boolean>> query = ww => (ww.ID == resToken.LogFileID);
            var logFile = await logFileRepository.GetAsync(query);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
                return actionResponse.ToIActionResult();
            }

            actionResponse.SetFile(logFile.FileBytes ?? new Byte[0], "application/octet-stream", logFile.FileName ?? "ex.bin");
            return actionResponse.ToIActionResult();
        }
        [HttpGet("UsernameFromToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsernameFromToken([FromQuery] String? token, [FromServices] LogTokenRepository logTokenRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var resToken = await logTokenRepository.GetAsync(ww => String.Equals(ww.Token, token));
            if (resToken is null)
            {
                actionResponse.AddNotFoundErr("Token");
                return actionResponse.ToIActionResult();
            }
            if ((DateTime.UtcNow - resToken.CreateTime).TotalHours > Constants.LogReportTokenDurationHours)
            {
                actionResponse.AddExpiredErr("Token");
                return actionResponse.ToIActionResult();
            }
            actionResponse.SetData(new { name = resToken.Username });
            return actionResponse.ToIActionResult();
        }

        [HttpGet("DownloadResultReport")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadResultReport([FromQuery] Guid id, [FromServices] LogReportRepository logReportRepository,
        [FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] LogFileRepository logFileRepository, [FromServices] MyEmailService emailService)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var logReport = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (logReport is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
                return actionResponse.ToIActionResult();
            }
            var secondLogReport = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            var logResult = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (logResult is null)
            {
                actionResponse.AddNotFoundErr("ResultReport");
                return actionResponse.ToIActionResult();
            }
            StringBuilder html = emailService.GenerateResultLogReport(logResult, logReport, secondLogReport);
            var htmlString = html.ToString();

            Byte[]? bytes = null;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MemoryStream ms = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(htmlString, ms);
                bytes = ms.ToArray();
            }
            actionResponse.SetFile(bytes, "application/pdf", "report.pdf");
            // actionResponse.Data = htmlString;
            return actionResponse.ToIActionResult();
        }

        [HttpPost("{id:Guid}/CreateSecondReport")]
        public async Task<IActionResult> CreateSecondReport([FromRoute] Guid id, [FromBody] AddingSecondLogReportCommand command, [FromServices] LogFileRepository logFileRepository,
        [FromServices] MyEmailService emailService, [FromServices] IOptions<TargetEmailSettings> options,
        [FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] LogTokenRepository logTokenRepository, [FromServices] MyMinioService minioService)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            TargetEmailSettings settings = options.Value;
            if (CurrentUser.RoleID != 3 && !CurrentUser.IsAdministrator)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var logFile = await logFileRepository.GetAsync(ww => ww.ID == id);
            if (logFile is null)
            {
                response.AddNotFoundErr("LogFile");
                return response.ToIActionResult();
            }
            if (logFile.Status != LogStatus.SecondWarning)
            {
                response.AddInvalidErr("LogStatus");
            }
            var report = new SecondLogReport
            {
                LogFileID = id,
                AccidentTime = command.AccidentTime,
                ImageUrls = new List<String>(),
                PilotDescription = command.PilotDescription,
                ReporterDescription = command.ReporterDescription,
                Suggest = command.Suggest,
                UserUUID = CurrentUser.UUID,
                PilotName = command.PilotName,
                PartnerCompanyName = command.PartnerCompanyName,
                Username = CurrentUser.Email,
            };

            foreach (var url in command.ImageUrls)
            {
                var fileLink = await minioService.CopyObjectAsync(url, new String[] { "drone-hub-api", "second-log-reports", $"{logFile.ID}" });
                report.ImageUrls.Add(fileLink);
            }

            var secondReport = await secondLogReportRepository.CreateAsync(report);

            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            foreach (UserEmail item in settings.LogReport ?? new List<UserEmail>())
            {
                String newToken = TokenHelper.GenerateToken();
                await logTokenRepository.CreateAsync(new LogToken { Token = newToken, UserUUID = item.UUID != null ? new Guid(item.UUID) : Guid.Empty, LogFileID = logFile.ID, Username = item.Email });
                await emailService.SendMailAsync(new String[] { item.Email ?? "" }, new String[] { }, new String[] { }, @$"[Chuyến bay cần phân tích lần 2] Mã hiệu drone ({logFile.Device?.Name ?? ""})",
                $"Dear,\n\nPhòng Đặc Nhiệm trả kết quả báo cáo hiện tường:\n\nMã hiệu Drone: {logFile.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(logFile.LoggingTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nLink Báo cáo tình trạng chuyến bay: https://dronehub.mismart.ai/second-log-result?token={newToken} \n\nThank you");
            }

            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UploadSecondReportImage")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> UploadSecondReportImage([FromRoute] Guid id, [FromServices] SecondLogReportRepository secondLogReportRepository, [FromServices] MyMinioService minioService, [FromForm] AddingLogImageLinkCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var report = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            // Console.WriteLine(report);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
                return actionResponse.ToIActionResult();
            }
            if (command.Files != null)
            {
                if (report.ImageUrls is null) report.ImageUrls = new List<String>();
                for (var i = 0; i < command.Files.Count; i++)
                {
                    var fileLink = await minioService.PutFileAsync(command.Files[i], new String[] { "drone-hub-api", "second-log-reports", $"{id}" });
                    report.ImageUrls.Add(fileLink);
                }
                await secondLogReportRepository.UpdateAsync(report);
            }

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("{id:Guid}/AnalystError")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> AnalystError([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] MyEmailService emailService, [FromBody] AddingLogErrorCommand command, [FromServices] LogFileRepository logFileRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var result = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (result is null)
            {
                actionResponse.AddNotFoundErr("ResultReport");
                return actionResponse.ToIActionResult();
            }
            if (result.LogFile != null && result.LogFile.Status != LogStatus.Completed)
            {
                actionResponse.AddInvalidErr("LogStatus");
                return actionResponse.ToIActionResult();
            }

            var logFile = await logFileRepository.GetAsync(ww => ww.ID == result.LogFileID);
            if (logFile is null)
            {
                actionResponse.AddNotFoundErr("LogFile");
                return actionResponse.ToIActionResult();
            }
            logFile.Status = LogStatus.Completed;
            await logFileRepository.UpdateAsync(logFile);
            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            if (result.AnalystName != null && result.LogFile?.LoggingTime != null)
                await emailService.SendMailAsync(new String[] { result.AnalystName }, new String[] { }, new String[] { }, @$"[Báo cáo cần chỉnh sửa] Mã hiệu drone ({result.LogFile?.Device?.Name ?? ""})",
                $"Dear,\n\nYêu cầu vào trang Drone Hub chỉnh sửa\n\nMã hiệu Drone: {result.LogFile?.Device?.Name ?? ""}\n\nMã chuyến bay: {logFile.FlightID}\n\nThời gian ghi log: {TimeZoneInfo.ConvertTimeFromUtc(result.LogFile?.LoggingTime ?? DateTime.UtcNow, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss")}\n\nGhi chú: {command.Message}\n\nThank you");

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UpdateImageUrlsReport")]
        public async Task<IActionResult> UpdateImageUrlsReport([FromRoute] Guid id, [FromServices] LogReportRepository logReportRepository,
        [FromServices] MyEmailService emailService, [FromBody] UpdateImageUrlsCommand command, [FromServices] MyMinioService minioService)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (CurrentUser.RoleID != 3 && !CurrentUser.IsAdministrator)
            {
                actionResponse.AddNotAllowedErr();
                return actionResponse.ToIActionResult();
            }
            var report = await logReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("LogReport");
                return actionResponse.ToIActionResult();
            }
            if (report.LogFile != null && report.LogFile.Status != LogStatus.Warning)
            {
                actionResponse.AddInvalidErr("LogStatus");
                return actionResponse.ToIActionResult();
            }
            if (report.ImageUrls != null)
            {
                for (var i = 0; i < report.ImageUrls.Count; i++)
                {
                    String url = report.ImageUrls[i];
                    if (!command.ImageUrls.Contains(url))
                        await minioService.RemoveFileByUrlAsync(url);
                }
                report.ImageUrls = command.ImageUrls;

                await logReportRepository.UpdateAsync(report);
            }

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("{id:Guid}/UpdateImageUrlsResult")]

        public async Task<IActionResult> UpdateImageUrlsResult([FromRoute] Guid id, [FromServices] LogReportResultRepository logReportResultRepository,
        [FromServices] MyEmailService emailService, [FromBody] UpdateImageUrlsCommand command, [FromServices] MyMinioService minioService)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (CurrentUser.RoleID != 4)
            {
                actionResponse.AddNotAllowedErr();
            }
            var result = await logReportResultRepository.GetAsync(ww => ww.LogFileID == id);
            if (result is null)
            {
                actionResponse.AddNotFoundErr("ResultReport");
                return actionResponse.ToIActionResult();
            }
            if (result.LogFile != null && result.LogFile.Status != LogStatus.Completed)
            {
                actionResponse.AddInvalidErr("LogStatus");
                return actionResponse.ToIActionResult();
            }
            if (result.ImageUrls != null)
            {
                for (var i = 0; i < result.ImageUrls.Count; i++)
                {
                    String url = result.ImageUrls[i];
                    if (!command.ImageUrls.Contains(url))
                        await minioService.RemoveFileByUrlAsync(url);
                }
                result.ImageUrls = command.ImageUrls;

                await logReportResultRepository.UpdateAsync(result);
            }

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("{id:Guid}/UpdateImageUrlsSecondReport")]
        public async Task<IActionResult> UpdateImageUrlsSecondReport([FromRoute] Guid id, [FromServices] SecondLogReportRepository secondLogReportRepository,
        [FromServices] MyEmailService emailService, [FromBody] UpdateImageUrlsCommand command, [FromServices] MyMinioService minioService)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (CurrentUser.RoleID != 3 && !CurrentUser.IsAdministrator)
            {
                actionResponse.AddNotAllowedErr();
            }
            var report = await secondLogReportRepository.GetAsync(ww => ww.LogFileID == id);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("SecondReport");
                return actionResponse.ToIActionResult();
            }
            if (report.LogFile != null && report.LogFile.Status != LogStatus.SecondWarning)
            {
                actionResponse.AddInvalidErr("LogStatus");
                return actionResponse.ToIActionResult();

            }
            if (report.ImageUrls != null)
            {
                for (var i = 0; i < report.ImageUrls.Count; i++)
                {
                    String url = report.ImageUrls[i];
                    if (!command.ImageUrls.Contains(url))
                        await minioService.RemoveFileByUrlAsync(url);
                }
                report.ImageUrls = command.ImageUrls;

                await secondLogReportRepository.UpdateAsync(report);
            }

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
    }
}