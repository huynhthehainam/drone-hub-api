using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class StreamingLink : EntityBase<Int32>
    {
        public StreamingLink()
        {
        }

        public StreamingLink(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private Device? device;
        public Device? Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }

        public Int32 DeviceID { get; set; }
        public String? Link { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}