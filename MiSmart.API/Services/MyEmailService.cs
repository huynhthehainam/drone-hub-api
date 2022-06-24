

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using MiSmart.DAL.Models;
using System;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.Settings;

namespace MiSmart.API.Services;

public class MyEmailService : EmailService
{
    private readonly TargetEmailSettings targetEmailSettings;
    private readonly FrontEndSettings frontEndSettings;
    public MyEmailService(IOptions<SmtpSettings> options, IOptions<EmailSettings> options1, IOptions<TargetEmailSettings> options2, IOptions<FrontEndSettings> options3) : base(options, options1)
    {
        this.targetEmailSettings = options2.Value;
        this.frontEndSettings = options3.Value;
    }
    public async Task SendLowBatteryReport(FlightStat stat, Boolean isOnline)
    {
        var online = isOnline ? "online" : "offline";
        await SendMailAsync(targetEmailSettings.LowBattery.ToArray(), new String[] { }, new String[] { }, "Báo cáo chuyến bay phần trăm Pin tháp", @$"
                                    diện tích: {(stat.TaskArea / 10000).ToString("0.##")} ha,
                                    thời điểm: {stat.FlightTime}
                                    thiết bị: {stat.Device.Name}
                                    thời gian bay: {stat.FlightDuration.ToString("0.##")} giờ
                                    id: {stat.ID}
                                    phần trăm pin còn lại: {stat.BatteryPercentRemaining.GetValueOrDefault()}%
                                    link: {frontEndSettings.Domain}/apps/execution-flight-statistics/map/{stat.ID}
                                    {online}
                            ");
    }
}