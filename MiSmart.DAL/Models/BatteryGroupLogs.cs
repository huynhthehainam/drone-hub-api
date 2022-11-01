

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class BatteryGroupLog : EntityBase<Guid>
    {
        public BatteryGroupLog()
        {
        }

        public BatteryGroupLog(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private ICollection<BatteryLog>? logs;
        public ICollection<BatteryLog>? Logs
        {
            get => lazyLoader.Load(this, ref logs);
            set => logs = value;
        }



        private Battery? battery;
        public Battery? Battery
        {
            get => lazyLoader.Load(this, ref battery);
            set => battery = value;
        }
        public Int32 BatteryID { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;


        private ICollection<Battery>? lastBatteries;
        public ICollection<Battery>? LastBatteries
        {
            get => lazyLoader.Load(this, ref lastBatteries);
            set => lastBatteries = value;
        }
    }
}