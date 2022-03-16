


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
        public String FileUrl { get; set; }
        private Device device;
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
    }
}