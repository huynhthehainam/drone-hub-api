

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using NetTopologySuite.Geometries;
using MiSmart.Infrastructure.Constants;

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

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public Point LocationPoint { get; set; }
        public Double Direction { get; set; }
        public String AdditionalInformationString { get; set; }
        [NotMapped]
        public Object AdditionalInformation
        {
            get => JsonSerializer.Deserialize<Object>(AdditionalInformationString, JsonSerializerDefaultOptions.CamelOptions);
            set => AdditionalInformationString = JsonSerializer.Serialize(value, JsonSerializerDefaultOptions.CamelOptions);
        }

        private TelemetryGroup group;
        public TelemetryGroup Group
        {
            get => lazyLoader.Load(this, ref group);
            set => group = value;
        }
        public Int64 GroupID { get; set; }
    }
}