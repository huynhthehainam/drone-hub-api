

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public String AdditionalInformationString { get; set; }
        [NotMapped]
        public Object AdditionalInformation
        {
            get => JsonSerializer.Deserialize<Object>(AdditionalInformationString);
            set => AdditionalInformationString = JsonSerializer.Serialize(value);
        }

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