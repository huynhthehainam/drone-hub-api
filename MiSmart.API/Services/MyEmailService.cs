

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using MiSmart.DAL.Models;
using System;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.Settings;
using System.IO;

namespace MiSmart.API.Services;

public class MyEmailService : EmailService
{

    private const String htmlFolderPath = "./HTMLTemplates"; private readonly TargetEmailSettings targetEmailSettings;
    private readonly FrontEndSettings frontEndSettings;
    public MyEmailService(IOptions<SmtpSettings> options, IOptions<EmailSettings> options1, IOptions<TargetEmailSettings> options2, IOptions<FrontEndSettings> options3) : base(options, options1)
    {
        this.targetEmailSettings = options2.Value;
        this.frontEndSettings = options3.Value;
    }

    private String getHTML(String name)
    {
        var path = $"{htmlFolderPath}/{name}.html";
        if (File.Exists(path))
        {
            var html = File.ReadAllText(path);
            return html;
        }
        return null;
    }
    private String generateLowBatteryReportHTML(String problemStatus, DateTime flightTime, String pilotName, String teamName, String pilotPhone, String pilotEmail, String deviceName, String customerName, String locationName, Double batteryPercent, String reportLink, String documentLink)
    {
        var html = getHTML("LowBatteryReport");
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
    public async Task SendLowBatteryReport(FlightStat stat, Boolean isOnline)
    {
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
            stat.Device.Name,
            "",
            stat.TaskLocation,
            stat.BatteryPercentRemaining.GetValueOrDefault(),
            $"{frontEndSettings.Domain}/apps/execution-flight-statistics/map/{stat.ID}",
            ""
        );
        await SendMailAsync(targetEmailSettings.LowBattery.ToArray(), new String[] { }, new String[] { }, "Báo cáo chuyến bay phần trăm Pin thấp", html, true);
    }
}