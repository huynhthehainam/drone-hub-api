

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json;
using MiSmart.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiSmart.DAL.Models
{
    public class GPSPoint
    {
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public Double Accuracy { get; set; }
        public Double Yaw { get; set; }
    }
    public class LocationPoint
    {
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LengthUnit
    {
        Meter,
        Feet
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AreaUnit
    {
        SquareMeter,
        Hectare,
        Acre,
        Fen,
        Rai
    }
    public class Field : EntityBase<Int64>
    {
        public Field() : base() { }
        public Field(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public String Name { get; set; }
        public string CreatedTime { get; set; }
        public String FieldName { get; set; }
        public String FieldLocation { get; set; }
        public String PilotName { get; set; }
        public Double MappingArea { get; set; }
        public AreaUnit Unit { get; set; } = AreaUnit.Hectare;
        public Double MappingTime { get; set; } 
        public DateTime? UpdatedTime { get; set; } = null;

        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }

        public String EdgedLocationPointsString { get; set; }
        [NotMapped]
        public List<LocationPoint> EdgedLocationPoints
        {
            get => JsonSerializer.Deserialize<List<LocationPoint>>(EdgedLocationPointsString);
            set => EdgedLocationPointsString = JsonSerializer.Serialize(value);
        }

        public String FlywayPointsString { get; set; }
        [NotMapped]
        public List<LocationPoint> FlywayPoints
        {
            get => JsonSerializer.Deserialize<List<LocationPoint>>(FlywayPointsString);
            set => FlywayPointsString = JsonSerializer.Serialize(value);
        }
        public String GPSPointsString { get; set; }
        [NotMapped]
        public List<GPSPoint> GPSPoints
        {
            get => JsonSerializer.Deserialize<List<GPSPoint>>(GPSPointsString);
            set => GPSPointsString = JsonSerializer.Serialize(value);
        }

        public String LocationPointString { get; set; }
        [NotMapped]
        public LocationPoint locationPoint
        {
            get => JsonSerializer.Deserialize<LocationPoint>(LocationPointString);
            set => LocationPointString = JsonSerializer.Serialize(value);
        }

        public Double WorkSpeed { get; set; }
        public Double WorkArea { get; set; }
        public Double InnerArea { get; set; }
        public Double SprayWidth { get; set; }
        public Double SprayDir { get; set; }
        public Boolean IsLargeFarm { get; set; } = false;
        public Double EdgeOffset { get; set; }
        private String CalibrationPointsString { get; set; }
        [NotMapped]
        public List<LocationPoint> CalibrationPoints
        {
            get => JsonSerializer.Deserialize<List<LocationPoint>>(CalibrationPointsString);
            set => CalibrationPointsString = JsonSerializer.Serialize(value);
        }
    }
}