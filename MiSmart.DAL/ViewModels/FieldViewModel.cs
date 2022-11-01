using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class FieldViewModel : IViewModel<Field>
    {
        public Int64 ID { get; set; }
        public List<Object>? CalibrationPoints { get; set; }
        public DateTime CreatedTime { get; set; }
        public Int32 CustomerID { get; set; }
        // public List<LocationPoint> EdgedLocationPoints { get; set; }
        public Double EdgeOffset { get; set; }
        public String? FieldLocation { get; set; }
        public String? FieldName { get; set; }
        // public List<LocationPoint> FlywayPoints { get; set; }
        // public List<GPSPoint> GPSPoints { get; set; }
        public Double InnerArea { get; set; }
        public Boolean IsLargeFarm { get; set; }
        // public LocationPoint LocationPoint { get; set; }
        public Double MappingArea { get; set; }
        public Double MappingTime { get; set; }
        public String? Name { get; set; }
        public String? PilotName { get; set; }
        public Double SprayDir { get; set; }
        public Double SprayWidth { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public Double WorkArea { get; set; }
        public Double WorkSpeed { get; set; }

        public void LoadFrom(Field entity)
        {
            ID = entity.ID;
            // CalibrationPoints = entity.CalibrationPoints;
            CreatedTime = entity.CreatedTime;
            CustomerID = entity.CustomerID;
            // EdgedLocationPoints = entity.EdgedLocationPoints;
            EdgeOffset = entity.EdgeOffset;
            FieldLocation = entity.FieldLocation;
            FieldName = entity.FieldName;
            // FlywayPoints = entity.FlywayPoints;
            // GPSPoints = entity.GPSPoints;
            InnerArea = entity.InnerArea;
            IsLargeFarm = entity.IsLargeFarm;
            // LocationPoint = entity.LocationPoint;
            MappingArea = entity.MappingArea;
            MappingTime = entity.MappingTime;
            Name = entity.Name;
            PilotName = entity.PilotName;
            SprayDir = entity.SprayDir;
            SprayWidth = entity.SprayWidth;
            UpdatedTime = entity.UpdatedTime;
            WorkArea = entity.WorkArea;
            WorkSpeed = entity.WorkSpeed;
        }
    }
}