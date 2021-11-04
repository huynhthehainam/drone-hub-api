using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiSmart.DAL.Models
{
    public class FlightStat : EntityBase<Guid>
    {
        public FlightStat() : base()
        {
        }

        public FlightStat(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime FlightTime { get; set; } = DateTime.Now;
        public String TaskLocation { get; set; }
        public Int32 Flights { get; set; }
        public String FieldName { get; set; }
        public String DeviceName { get; set; }
        public Double TaskArea { get; set; }
        public AreaUnit TaskAreaUnit { get; set; } = AreaUnit.Hectare;
        public Double FlightDuration { get; set; }
        public String PilotName { get; set; }
        public String FlywayPointsString { get; set; }
        [NotMapped]
        public List<LocationPoint> FlywayPoints
        {
            get => JsonSerializer.Deserialize<List<LocationPoint>>(FlywayPointsString);
            set => FlywayPointsString = JsonSerializer.Serialize(value);
        }
        private Device device;
        [JsonIgnore]
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }

        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }
    }
}