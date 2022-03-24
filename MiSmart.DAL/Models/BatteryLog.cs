


using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class BatteryLog : EntityBase<Guid>
    {
        public BatteryLog()
        {
        }

        public BatteryLog(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        private BatteryGroupLog groupLog;
        public BatteryGroupLog GroupLog
        {
            get => lazyLoader.Load(this, ref groupLog);
            set => groupLog = value;
        }
        public Guid GroupLogID { get; set; }


        public Double PercentRemaining { get; set; }
        public Double Temperature { get; set; }
        public String TemperatureUnit { get; set; }
        public Double CellMinimumVoltage { get; set; }
        public String CellMinimumVoltageUnit { get; set; }
        public Double CellMaximumVoltage { get; set; }
        public String CellMaximumVoltageUnit { get; set; }
        public Int32 CycleCount { get; set; }
        public Double Current { get; set; }
        public String CurrentUnit { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;


    }
}