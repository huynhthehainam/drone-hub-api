using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

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
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime FlightTime { get; set; } = DateTime.Now;
        public String TaskLocation { get; set; }
        public Int32 Flights { get; set; }
        public String FieldName { get; set; }
        public String DeviceName { get; set; }
        public Double TaskArea { get; set; }
        public Double FlightDuration { get; set; }
        public String PilotName { get; set; }
        public LineString FlywayPoints { get; set; }
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

        private ExecutionCompany executionCompany;
        public ExecutionCompany ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32 ExecutionCompanyID { get; set; }
    }
}