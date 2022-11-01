

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class MaintenanceReport : EntityBase<Int32>
    {

        public MaintenanceReport(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public MaintenanceReport()
        {
        }
        public String? Reason { get; set; }
        public List<String>? AttachmentLinks { get; set; }

        private Device? device;
        public Device ?Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime ActualReportCreatedTime { get; set; } = DateTime.UtcNow;
        public Guid UserUUID { get; set; }
        public Guid UUID { get; set; } = Guid.NewGuid();
    }
}