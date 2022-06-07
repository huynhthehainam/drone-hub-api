

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System.Text.Json;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.Models
{
    public class TelemetryRecord : EntityBase<Guid>
    {
        public TelemetryRecord() : base()
        {
        }

        public TelemetryRecord(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public Point LocationPoint { get; set; }
        public Double Direction { get; set; }
        public JsonDocument AdditionalInformation
        {
            get; set;
        }

        private TelemetryGroup group;
        public TelemetryGroup Group
        {
            get => lazyLoader.Load(this, ref group);
            set => group = value;
        }
        public Guid GroupID { get; set; }
    }
}