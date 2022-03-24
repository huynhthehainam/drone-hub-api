using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class Battery : EntityBase<Int32>
    {
        public Battery()
        {
        }

        public Battery(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public String ActualID { get; set; }



        private ICollection<BatteryGroupLog> groupLogs;
        public ICollection<BatteryGroupLog> GroupLogs
        {
            get => lazyLoader.Load(this, ref groupLogs);
            set => groupLogs = value;
        }
    }
}