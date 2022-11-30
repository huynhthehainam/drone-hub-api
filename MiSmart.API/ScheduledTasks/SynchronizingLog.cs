using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ScheduledTasks;
using Renci.SshNet;

namespace MiSmart.API.ScheduledTasks
{
    public class SynchronizingLog : CronJobService
    {
        private IServiceProvider serviceProvider;
        private const String logFolder = "/home/ubuntu/official_rsync";
        private const String binFilePatternString = "^(?<order>[0-9]+[0-9]+)-(?<year>[0-9]+[0-9]+)-(?<month>[0-9]+[0-9]+)-(?<day>[0-9]+[0-9]+)_(?<hour>[0-9]+[0-9]+)-(?<minute>[0-9]+[0-9]+)-(?<second>[0-9]+[0-9]+).bin$";
        public SynchronizingLog(IScheduleConfig<SynchronizingLog> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            Regex binFileRegex = new Regex(binFilePatternString);
            TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            using (var scope = serviceProvider.CreateScope())
            {
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    PrivateKeyFile keyFile = new PrivateKeyFile("svgh-mismart.pem");
                    Console.WriteLine($"Start download file {DateTime.UtcNow.ToString()}");
                    using (SftpClient client = new SftpClient("ec2-13-212-215-73.ap-southeast-1.compute.amazonaws.com", "ubuntu", new[] { keyFile }))
                    {
                        client.Connect();
                        var folders = client.ListDirectory(logFolder);
                        foreach (var folder in folders)
                        {
                            if (folder.Name != "." && folder.Name != "..")
                            {
                                var device = databaseContext.Devices.Where(ww => ww.Token == folder.Name).FirstOrDefault();
                                if (device is not null)
                                {
                                    var deviceFolder = Path.Join(logFolder, folder.Name);
                                    var logFiles = client.ListDirectory(deviceFolder);
                                    List<DateTime> times = new List<DateTime>();
                                    foreach (var logFile in logFiles)
                                    {
                                        var m = binFileRegex.Match(logFile.Name);
                                        if (m.Success)
                                        {
                                            var existedDBLogFile = databaseContext.LogFiles.Where(ww => ww.FileName == logFile.Name && ww.DeviceID == device.ID).FirstOrDefault();
                                            if (existedDBLogFile is null)
                                            {
                                                var order = Convert.ToInt32(m.Groups["order"].ToString());
                                                var year = Convert.ToInt32(m.Groups["year"].ToString());
                                                var month = Convert.ToInt32(m.Groups["month"].ToString());
                                                var day = Convert.ToInt32(m.Groups["day"].ToString());
                                                var hour = Convert.ToInt32(m.Groups["hour"].ToString());
                                                var minute = Convert.ToInt32(m.Groups["minute"].ToString());
                                                var second = Convert.ToInt32(m.Groups["second"].ToString());
                                                var time = new DateTime(year, month, day, hour, minute, second);
                                                var utcTime = TimeZoneInfo.ConvertTimeToUtc(time, seaTimeZone);
                                                var logPath = Path.Join(deviceFolder, logFile.Name);
                                                times.Add(utcTime);
                                                MemoryStream ms = new MemoryStream();
                                                try
                                                {
                                                    client.DownloadFile(logPath, ms);
                                                    Console.WriteLine($"Download file: {logPath}");
                                                    var byteArr = ms.ToArray();
                                                    var dbLogFile = new LogFile() { DeviceID = device.ID, FileBytes = byteArr, LoggingTime = utcTime, FileName = logFile.Name, FlightID = order };
                                                    databaseContext.LogFiles.Add(dbLogFile);
                                                    databaseContext.SaveChanges();
                                                }
                                                catch (Exception)
                                                {
                                                    Console.WriteLine("Cannot download file from scp");
                                                }
                                            }
                                            else
                                            {
                                                times.Add(existedDBLogFile.LoggingTime);
                                                var now = DateTime.UtcNow.AddDays(-1);
                                                if (existedDBLogFile.LoggingTime > now)
                                                {

                                                    var logPath = Path.Join(deviceFolder, logFile.Name);
                                                    MemoryStream ms = new MemoryStream();
                                                    try
                                                    {
                                                        client.DownloadFile(logPath, ms);
                                                        var byteArr = ms.ToArray();
                                                        existedDBLogFile.FileBytes = byteArr;
                                                        databaseContext.Update(existedDBLogFile);
                                                        databaseContext.SaveChanges();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine("Cannot download file from scp");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (times.Count > 0)
                                    {
                                        times.Sort();
                                        Console.WriteLine($"Latest time  {times.LastOrDefault()}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}