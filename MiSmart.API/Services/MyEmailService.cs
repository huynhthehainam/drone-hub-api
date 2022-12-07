

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using MiSmart.DAL.Models;
using System;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.Settings;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;
using MiSmart.Infrastructure.Extensions;

namespace MiSmart.API.Services;


public class MyEmailService : EmailService
{

    private const String htmlFolderPath = "./HTMLTemplates";
    private readonly TargetEmailSettings targetEmailSettings;
    private readonly FrontEndSettings frontEndSettings;
    private readonly SmtpSettings smtpSettings;
    private readonly EmailSettings emailSettings;
    private readonly IWebHostEnvironment env;

    public MyEmailService(IOptions<SmtpSettings> options, IOptions<EmailSettings> options1, IOptions<TargetEmailSettings> options2, IOptions<FrontEndSettings> options3, IWebHostEnvironment env) : base(options, options1)
    {
        this.emailSettings = options1.Value;
        this.smtpSettings = options.Value;
        this.targetEmailSettings = options2.Value;
        this.frontEndSettings = options3.Value;
        this.env = env;
    }

    public String? GetHTML(String name)
    {
        var path = $"{htmlFolderPath}/{name}.html";
        if (File.Exists(path))
        {
            var html = File.ReadAllText(path);
            return html;
        }
        return null;
    }
    private String? generateLowBatteryReportHTML(String problemStatus, DateTime flightTime, String pilotName, String teamName, String pilotPhone, String pilotEmail, String deviceName, String customerName, String locationName, Double batteryPercent, String reportLink, String documentLink)
    {

        var html = GetHTML("LowBatteryReport");
        if (html == null)
        {
            return html;
        }
        html = html.Replace("problem_status_indicator", problemStatus);
        html = html.Replace("flight_time_indicator", flightTime.ToString());
        html = html.Replace("pilot_name_indicator", pilotName);
        html = html.Replace("team_name_indicator", teamName);
        html = html.Replace("pilot_phone_indicator", pilotPhone);
        html = html.Replace("pilot_email_indicator", pilotEmail);
        html = html.Replace("device_name_indicator", deviceName);
        html = html.Replace("customer_name_indicator", customerName);
        html = html.Replace("location_name_indicator", locationName);
        html = html.Replace("battery_percent_indicator", batteryPercent.ToString());
        html = html.Replace("report_link_indicator", reportLink);
        html = html.Replace("document_link_indicator", documentLink);
        return html;
    }
    public async Task SendLowBatteryReportAsync(FlightStat stat, Boolean isOnline)
    {
        if (env.IsTesting())
            return;
        var online = isOnline ? "online" : "offline";
        TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var localFlightTime = TimeZoneInfo.ConvertTimeFromUtc(stat.FlightTime, seaTimeZone);
        var html = generateLowBatteryReportHTML(
            "Nghiêm trọng",
            localFlightTime,
            "",
            "",
            "",
            "",
            stat.Device?.Name ?? "",
            "",
            stat.TaskLocation ?? "",
            stat.BatteryPercentRemaining.GetValueOrDefault(),
            $"{frontEndSettings.Domain}/apps/execution-flight-statistics/map/{stat.ID}",
            ""
        );
        var rawTargetEmails = targetEmailSettings.LowBattery ?? new List<String>();
        var executionCompanyTargetEmails = stat.Device?.ExecutionCompany?.Emails ?? new List<String>();
        rawTargetEmails.AddRange(executionCompanyTargetEmails);
        rawTargetEmails.Distinct();
        var targetEmails = new List<String>();
        foreach (var email in rawTargetEmails)
        {
            if (!targetEmails.Contains(email))
            {
                targetEmails.Add(email);
            }
        }
        await SendMailAsync(targetEmails.ToArray(), new String[] { }, new String[] { }, $"[{stat.Device?.Name ?? ""}] Báo cáo chuyến bay phần trăm Pin thấp", html ?? "", true);
    }
    private String? generateLowBatteryDailyReport(List<FlightStat> listStat, DateTime localNow)
    {
        var tableData = "";
        TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        for (Int32 i = 0; i < listStat.Count; i++)
        {
            var flightStat = listStat[i];
            var row = GetHTML("LowBatteryRow");
            if (row == null)
            {
                continue;
            }
            var localFlightTime = TimeZoneInfo.ConvertTimeFromUtc(flightStat.FlightTime, seaTimeZone);
            row = row.Replace("index", (i + 1).ToString());
            row = row.Replace("time", localFlightTime.ToString());
            row = row.Replace("pilot", "");
            row = row.Replace("drone", flightStat.DeviceName);
            row = row.Replace("customer", "");
            row = row.Replace("location", flightStat.TaskLocation);
            row = row.Replace("percent_battery", flightStat.BatteryPercentRemaining.ToString() + "%");
            tableData += row;
        }

        var html = GetHTML("LowBatteryDailyReport");
        if (html is null)
        {
            return html;
        }
        html = html.Replace("table_data_indicator ", tableData);

        html = html.Replace("DDMMYYYY", localNow.ToString("yyyy-MM-dd"));
        return html;
    }

    public async Task SendLowBatteryDailyReportAsync(List<FlightStat> flightStats, DateTime localNow)
    {
        if (env.IsTesting())
            return;
        var html = generateLowBatteryDailyReport(flightStats, localNow);

        await SendMailAsync(targetEmailSettings.DailyReport?.ToArray() ?? new String[0], new String[] { }, new String[] { }, "Báo cáo cảnh báo vận hành pin sai cách " + localNow.ToString("yyyy-MM-dd"), html ?? "", true);
    }

    private String? convertImgToBase64(String name)
    {
        var path = $"{htmlFolderPath}/Image/{name}";
        if (File.Exists(path))
        {
            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    String base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }
        return null;
    }
    public StringBuilder GenerateResultLogReport(LogReportResult logResult, LogReport logReport, SecondLogReport? secondLogReport)
    {
        TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var html = GetHTML("ResultLogReport");
        StringBuilder htmlStringBuilder = new StringBuilder(html);

        String conclusionResult = logResult.Conclusion != null ? logResult.Conclusion.Replace(System.Environment.NewLine, "<br>") : "";
        conclusionResult = conclusionResult.Replace("\t", "&#09;");

        String suggestResult = logResult.Suggest != null ? logResult.Suggest.Replace(System.Environment.NewLine, "<br>") : "";
        suggestResult = suggestResult.Replace("\t", "&#09;");

        String detailAnalysisResult = logResult.DetailedAnalysis != null ? logResult.DetailedAnalysis.Replace(System.Environment.NewLine, "<br>") : "";
        detailAnalysisResult = detailAnalysisResult.Replace("\t", "&#09;");

        htmlStringBuilder.Replace("analysis_day", logResult.UpdatedTime.Day.ToString());
        htmlStringBuilder.Replace("analysis_month", logResult.UpdatedTime.Month.ToString());
        htmlStringBuilder.Replace("analysis_year", logResult.UpdatedTime.Year.ToString());
        htmlStringBuilder.Replace("updated_time", TimeZoneInfo.ConvertTimeFromUtc(logResult.UpdatedTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss"));

        String? sign = convertImgToBase64("signDat.png");
        if (sign != null)
            htmlStringBuilder.Replace("manager_sign", $"data:image/png;base64, {sign}");

        sign = convertImgToBase64("signAToan.png");
        if (sign != null)
            htmlStringBuilder.Replace("warden_sign", $"data:image/png;base64, {sign}");

        sign = convertImgToBase64("white.png");
        if (sign != null)
            htmlStringBuilder.Replace("analysis_sign", $"data:image/png;base64, {sign}");

        if (secondLogReport is null)
        {
            htmlStringBuilder.Replace("flight_id", "Chưa định danh");
            htmlStringBuilder.Replace("reporter_name", logReport.Username);
            htmlStringBuilder.Replace("drone_id", logReport.LogFile?.Device?.Name ?? "Không có thông tin");
            htmlStringBuilder.Replace("flight_location", logReport.LogFile?.LogDetail?.Location ?? "Không có thông tin");
            htmlStringBuilder.Replace("accident_time", TimeZoneInfo.ConvertTimeFromUtc(logReport.AccidentTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss AA"));
            htmlStringBuilder.Replace("pilot_name", logReport.PilotName);
            htmlStringBuilder.Replace("partner_company", logReport.LogFile?.Device?.ExecutionCompany?.Name ?? "Không có thông tin");
            htmlStringBuilder.Replace("pilot_description", logReport.PilotDescription);
            htmlStringBuilder.Replace("reporter_description", logReport.ReporterDescription);
            var listImageReport = "";
            if (logReport.ImageUrls != null)
                foreach (var imageUrl in logReport.ImageUrls)
                {
                    var item = GetHTML("ImageItem");
                    if (item != null)
                    {
                        item = item.Replace("img_src", imageUrl);
                        if (item != null)
                            listImageReport += item;
                    }
                }
            htmlStringBuilder.Replace("list_image_report", listImageReport);

            if (logResult.ResponsibleCompany == ResponsibleCompany.MiSmart)
                htmlStringBuilder.Replace("responsible_company", "MiSmart");
            else if (logResult.ResponsibleCompany == ResponsibleCompany.NoCompany)
                htmlStringBuilder.Replace("responsible_company", "Không có công ty");
            else if (logReport.LogFile != null && logReport.LogFile.Device != null && logReport.LogFile.Device.ExecutionCompany != null)
                htmlStringBuilder.Replace("responsible_company", logReport.LogFile.Device.ExecutionCompany?.Name);

        }
        else
        {
            htmlStringBuilder.Replace("flight_id", "Chưa định danh");
            htmlStringBuilder.Replace("reporter_name", secondLogReport.Username);
            htmlStringBuilder.Replace("drone_id", secondLogReport.LogFile?.Device?.Name ?? "Không có thông tin");
            htmlStringBuilder.Replace("flight_location", secondLogReport.LogFile != null ? secondLogReport.LogFile.LogDetail?.Location : "Không có thông tin");
            htmlStringBuilder.Replace("accident_time", TimeZoneInfo.ConvertTimeFromUtc(secondLogReport.AccidentTime, seaTimeZone).ToString("dd/MM/yyyy HH:mm:ss AA"));
            htmlStringBuilder.Replace("pilot_name", secondLogReport.PilotName);
            htmlStringBuilder.Replace("partner_company", secondLogReport.LogFile?.Device?.ExecutionCompany?.Name ?? "Không có thông tin");
            htmlStringBuilder.Replace("pilot_description", secondLogReport.PilotDescription);
            htmlStringBuilder.Replace("reporter_description", secondLogReport.ReporterDescription);
            var listImageReport = "";
            if (secondLogReport.ImageUrls != null)
                foreach (var imageUrl in secondLogReport.ImageUrls)
                {
                    var item = GetHTML("ImageItem");
                    if (item != null)
                    {
                        item = item.Replace("img_src", imageUrl);
                        if (item != null)
                            listImageReport += item;
                    }
                }
            htmlStringBuilder.Replace("list_image_report", listImageReport);

            if (logResult.ResponsibleCompany == ResponsibleCompany.MiSmart)
                htmlStringBuilder.Replace("responsible_company", "MiSmart");
            else if (logResult.ResponsibleCompany == ResponsibleCompany.NoCompany)
                htmlStringBuilder.Replace("responsible_company", "Không có công ty");
            else if (secondLogReport.LogFile != null && secondLogReport.LogFile.Device != null && secondLogReport.LogFile.Device.ExecutionCompany != null)
                htmlStringBuilder.Replace("responsible_company", secondLogReport.LogFile.Device.ExecutionCompany?.Name);

        }
        if (logResult.LogResultDetails != null)
        {
            var listError = logResult.LogResultDetails.ToArray();
            var tableData = "";
            for (Int32 i = 0; i < listError.Count(); i++)
            {
                var error = listError[i];
                if (error.PartError != null)
                {
                    var row = GetHTML("ResultDetailRow");
                    if (row == null)
                    {
                        continue;
                    }
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
            }
            htmlStringBuilder.Replace("table_data_indicator", tableData);
        }
        htmlStringBuilder.Replace("conclusion", conclusionResult);
        htmlStringBuilder.Replace("detailed_analysis", detailAnalysisResult);
        htmlStringBuilder.Replace("result_suggestion", suggestResult);
        htmlStringBuilder.Replace("by_analyst", logResult.AnalystName);
        htmlStringBuilder.Replace("by_approver", logResult.ApproverName);
        var listImageResult = "";
        if (logResult.ImageUrls != null)
            foreach (var ImageUrl in logResult.ImageUrls)
            {
                var item = GetHTML("ImageItem");
                if (item != null)
                {
                    item = item.Replace("img_src", ImageUrl);
                    if (item != null)
                        listImageResult += item;
                }
            }
        htmlStringBuilder.Replace("list_image_result", listImageResult);
        return htmlStringBuilder;
    }
    public async Task SendMailAttachmentAsync(String[] receivedUsers, String[] cCedUsers, String[] bCCedUsers, String subject, String body, String? senderName, String? from, Byte[] file, String? fileName, Boolean isBodyHtml = false)
    {
        if (env.IsTesting())
            return;

        senderName = senderName ?? emailSettings.SenderName;
        from = from ?? emailSettings.FromEmail;
        subject = subject ?? "";
        body = body ?? "";
        MailMessage mailMessage = new MailMessage();
        if (from != null)
        {
            if (String.IsNullOrWhiteSpace(senderName))
                mailMessage.From = new MailAddress(from);
            else
                mailMessage.From = new MailAddress(from, senderName);
        }
        foreach (var receivedUser in receivedUsers)
        {
            mailMessage.To.Add(receivedUser);
        }
        foreach (var cCedUser in cCedUsers)
        {
            mailMessage.CC.Add(cCedUser);
        }
        foreach (var bCCedUser in bCCedUsers)
        {
            mailMessage.CC.Add(bCCedUser);
        }
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = isBodyHtml;
        if (file is not null)
            mailMessage.Attachments.Add(new Attachment(new MemoryStream(file), fileName, "application/pdf"));
        using (var client = new SmtpClient(smtpSettings.Server))
        {
            client.Port = smtpSettings.Port;
            client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
            client.EnableSsl = smtpSettings.EnableSsl;
            await client.SendMailAsync(mailMessage);
        }

    }
}