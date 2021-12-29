

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json;
using MiSmart.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;


namespace MiSmart.DAL.Models
{

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
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public String FieldName { get; set; }
        public String FieldLocation { get; set; }
        public String PilotName { get; set; }
        public Double MappingArea { get; set; }
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
        public Polygon Border { get; set; }
        public LineString Flyway { get; set; }
        public MultiPoint GPSPoints { get; set; }
        public Point LocationPoint { get; set; }
        public MultiPoint CalibrationPoints { get; set; }

        public Double WorkSpeed { get; set; }
        public Double WorkArea { get; set; }
        public Double InnerArea { get; set; }
        public Double SprayWidth { get; set; }
        public Double SprayDir { get; set; }
        public Boolean IsLargeFarm { get; set; } = false;
        public Double EdgeOffset { get; set; }
    }
}