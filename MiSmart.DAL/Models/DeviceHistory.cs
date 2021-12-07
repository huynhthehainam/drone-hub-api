




using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.Models
{
    public class DeviceHistory : EntityBase<Int64>
    {
        public DeviceHistory() : base()
        {
        }

        public DeviceHistory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Point LocationPoint { get; set; }
        private Device device;
        [JsonIgnore]
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
    }
}