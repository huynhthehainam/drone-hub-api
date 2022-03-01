


using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace MiSmart.DAL.Models
{
    public class TelemetryGroup : EntityBase<Guid>
    {
        public TelemetryGroup() : base()
        {
        }


        public TelemetryGroup(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        private Device lastDevice;
        public Device LastDevice
        {
            get => lazyLoader.Load(this, ref lastDevice);
            set => lastDevice = value;
        }
        private ICollection<TelemetryRecord> records;
        public ICollection<TelemetryRecord> Records
        {
            get => lazyLoader.Load(this, ref records);
            set => records = value;
        }
        private Device device;
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}