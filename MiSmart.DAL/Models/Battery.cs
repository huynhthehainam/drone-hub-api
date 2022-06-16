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

        private BatteryModel batteryModel;
        public BatteryModel BatteryModel
        {
            get => lazyLoader.Load(this, ref batteryModel);
            set => batteryModel = value;
        }
        public Int32 BatteryModelID { get; set; }

        private ICollection<BatteryGroupLog> groupLogs;
        public ICollection<BatteryGroupLog> GroupLogs
        {
            get => lazyLoader.Load(this, ref groupLogs);
            set => groupLogs = value;
        }



        private BatteryGroupLog lastGroup;
        public BatteryGroupLog LastGroup
        {
            get => lazyLoader.Load(this, ref lastGroup);
            set => lastGroup = value;
        }
        public Guid? LastGroupID { get; set; }


        private ExecutionCompany executionCompany;
        public ExecutionCompany ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32? ExecutionCompanyID { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}