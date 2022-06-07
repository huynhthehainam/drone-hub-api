using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Text.Json;

namespace MiSmart.DAL.Models
{
    public class TMUser
    {
        public String ID { get; set; }
        public String UID { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
    }
    public class Medicine
    {
        public String ID { get; set; }
        public String Code { get; set; }
        public String Thumbnail { get; set; }
        public String Name { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlightMode
    {
        Spraying,
        Mapping,
        Seeding,
        Testing,
        Other,

    }
    public class FlightStat : EntityBase<Guid>
    {
        public FlightStat() : base()
        {
        }

        public FlightStat(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime FlightTime { get; set; } = DateTime.UtcNow;
        public String TaskLocation { get; set; }
        public Int32 Flights { get; set; }
        public String FieldName { get; set; }
        public String DeviceName { get; set; }
        public Double TaskArea { get; set; }
        public Double FlightDuration { get; set; }
        public String PilotName { get; set; }
        public String TMUserUID { get; set; }

        public JsonDocument TMUser
        {
            get; set;
        }
        public LineString FlywayPoints { get; set; }
        public List<Int32> SprayedIndexes { get; set; }
        private Device device;
        [JsonIgnore]
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }

        public Guid FlightUID { get; set; }

        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }

        private Team team;
        public Team Team
        {
            get => lazyLoader.Load(this, ref team);
            set => team = value;
        }
        public Int64? TeamID { get; set; }
        private ExecutionCompany executionCompany;
        public ExecutionCompany ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32? ExecutionCompanyID { get; set; }

        public FlightMode Mode { get; set; } = FlightMode.Spraying;


        public Double Cost { get; set; }
        private ICollection<ExecutionCompanyUserFlightStat> executionCompanyUserFlightStats;
        public ICollection<ExecutionCompanyUserFlightStat> ExecutionCompanyUserFlightStats
        {
            get => lazyLoader.Load(this, ref executionCompanyUserFlightStats);
            set => executionCompanyUserFlightStats = value;
        }

        public String MedicinesString { get; set; }

        public JsonDocument Medicines
        {
            get;
            set;
        }


        public JsonDocument AdditionalInformation
        {
            get;
            set;
        }
        public String GCSVersion { get; set; }
    }
}