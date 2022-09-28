using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;


namespace MiSmart.DAL.Models
{
    public class XYZ {
        public Double X {get; set; }
        public Double Y {get; set; }
        public Double Z {get; set; }
    }
    public class EdgeData {
        public Double Roll {get; set; }
        public Double Pitch {get; set; }
    }
    public class LogDetail : EntityBase<Int64>
    {
        public LogDetail() : base() { }
        public LogDetail(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public Guid LogFileID {get; set; }
        private LogFile logFile;
        public LogFile LogFile {
            get => lazyLoader.Load(this, ref logFile);
            set => logFile = value;
        }
        public Double FlightDuration { get; set; }
        public JsonDocument Vibe {get; set;}
        public Double PercentBattery { get; set; }
        public Double PercentFuel {get; set;}
        public JsonDocument Edge {get; set;}
        public Double BatteryCellDeviation {get; set;}
        public Double FlySpeed {get;set;}
        public Double Height {get; set;}
        public JsonDocument Accel {get;set;} 
    }
}