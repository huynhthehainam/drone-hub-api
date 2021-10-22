

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceStatus
    {
        Active,
        Inactive,

    }
    public class Device : EntityBase<Int32>
    {
        public Device() : base()
        {
        }

        public Device(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public String Name { get; set; }
        public Guid UUID { get; set; } = Guid.NewGuid();
        public String Token { get; set; } = TokenHelper.GenerateToken();
        private ICollection<TelemetryRecord> records;
        [JsonIgnore]
        public ICollection<TelemetryRecord> Records
        {
            get => lazyLoader.Load(this, ref records);
            set => records = value;
        }
        public DeviceStatus Status { get; set; } = DeviceStatus.Active;
        private Team team;
        [JsonIgnore]
        public Team Team
        {
            get => lazyLoader.Load(this, ref team);
            set => team = value;
        }
        public Int64? TeamID { get; set; }

        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }

        private ICollection<FlightStat> flightStats;
        [JsonIgnore]
        public ICollection<FlightStat> FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }
        private DeviceModel deviceModel;
        [JsonIgnore]
        public DeviceModel DeviceModel
        {
            get => lazyLoader.Load(this, ref deviceModel);
            set => deviceModel = value;
        }
        public Int32 DeviceModelID { get; set; }
        public LocationPoint LastLocation
        {
            get
            {
                var lastRecord = Records.OrderByDescending(ww => ww.CreatedTime).FirstOrDefault();
                if (lastRecord is not null)
                {
                    return new LocationPoint { Latitude = lastRecord.Latitude, Longitude = lastRecord.Longitude };
                }
                return null;
            }
        }
    }

}