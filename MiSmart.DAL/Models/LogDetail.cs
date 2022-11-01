using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;


namespace MiSmart.DAL.Models
{
    public class LogDetail : EntityBase<Int64>
    {
        public LogDetail() : base() { }
        public LogDetail(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public Guid LogFileID { get; set; }
        private LogFile? logFile;
        public LogFile? LogFile
        {
            get => lazyLoader.Load(this, ref logFile);
            set => logFile = value;
        }
        public Double FlightDuration { get; set; }
        public Double PercentBattery { get; set; }
        public Double PercentFuel { get; set; }
        public Double BatteryCellDeviation { get; set; }
        public Double FlySpeed { get; set; }
        public Double Height { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Boolean IsBingLocation { get; set; }
        public String? Location { get; set; }
        public Double VibeX { get; set; }
        public Double VibeY { get; set; }
        public Double VibeZ { get; set; }
        public Double Roll { get; set; }
        public Double Pitch { get; set; }
        public Double AccelX { get; set; }
        public Double AccelY { get; set; }
        public Double AccelZ { get; set; }
    }
}