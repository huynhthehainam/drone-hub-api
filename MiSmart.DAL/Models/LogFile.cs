


using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
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
    }
}