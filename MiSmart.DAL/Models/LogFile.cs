using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public enum DroneStatus
    {
        Stable,
        Unstable,
        Fall,
    }
    public enum LogStatus
    {
        NotReceive,
        Processing,
        Error,
        Approval,
    }
    public class LogFile : EntityBase<Guid>
    {
        public LogFile() : base()
        {
        }

        public LogFile(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        private Device device;
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
        public Byte[] FileBytes { get; set; }
        public String FileName { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime LoggingTime { get; set; } = DateTime.UtcNow;
        public DroneStatus DroneStatus { get; set; }
        public LogStatus Status {get; set; }
        public String[] Errors {get; set;}
        private LogDetail logDetail;
        public LogDetail LogDetail
        {
            get => lazyLoader.Load(this, ref logDetail);
            set => logDetail = value;
        }
        private LogReport logReport;
        public LogReport LogReport{
            get => lazyLoader.Load(this, ref logReport);
            set => logReport = value;
        }
        private LogReportResult logReportResult;
        public LogReportResult LogReportResult{
            get => lazyLoader.Load(this, ref logReportResult);
            set => logReportResult = value;
        } 
    }
}