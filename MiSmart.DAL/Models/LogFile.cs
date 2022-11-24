using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DroneStatus
    {
        Stable,
        Unstable,
        Fall,
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LogStatus
    {
        Normal,
        Warning,
        Completed,
        Approved,
        SecondWarning,
    }
    public class LogFile : EntityBase<Guid>
    {
        public LogFile() : base()
        {
        }

        public LogFile(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        private Device? device;
        public Device? Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
        public Byte[]? FileBytes { get; set; }
        public String? FileName { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime LoggingTime { get; set; } = DateTime.UtcNow;
        public DroneStatus DroneStatus { get; set; }
        public LogStatus Status { get; set; }
        private LogDetail? logDetail;
        public LogDetail? LogDetail
        {
            get => lazyLoader.Load(this, ref logDetail);
            set => logDetail = value;
        }
        private LogReport? logReport;
        public LogReport? LogReport
        {
            get => lazyLoader.Load(this, ref logReport);
            set => logReport = value;
        }
        private LogReportResult? logReportResult;
        public LogReportResult? LogReportResult
        {
            get => lazyLoader.Load(this, ref logReportResult);
            set => logReportResult = value;
        }
        private ICollection<LogToken>? logTokens;
        public ICollection<LogToken>? LogTokens
        {
            get => lazyLoader.Load(this, ref logTokens);
            set => logTokens = value;
        }
        public Boolean IsAnalyzed { get; set; } = false;
        private SecondLogReport? secondLogReport;
        public SecondLogReport? SecondLogReport
        {
            get => lazyLoader.Load(this, ref secondLogReport);
            set => secondLogReport = value;
        }


        public Double? DroneLogAnalyzingTaskID { get; set; }
        public DateTime? AnalyzingTime { get; set; }

    }
}