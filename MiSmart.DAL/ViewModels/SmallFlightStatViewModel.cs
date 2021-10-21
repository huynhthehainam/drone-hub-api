
using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallFlightStatViewModel : IViewModel<FlightStat>
    {
        public Guid ID { get; set; }
        public Double FlightDuration { get; set; }
        public DateTime CreateTime { get; set; }
        public Int32 CustomerID { get; set; }
        public Int32 DeviceID { get; set; }
        public Int32 Flights { get; set; }
        public DateTime FlightTime { get; set; }
        public String FieldName { get; set; }
        public List<LocationPoint> FlywayPoints { get; set; }
        public String TaskLocation { get; set; }
        public String TeamName { get; set; }
        public Double TaskArea { get; set; }
        public AreaUnit TaskAreaUnit { get; set; }
        public void LoadFrom(FlightStat entity)
        {
            ID = entity.ID;
            FlightDuration = entity.FlightDuration;
            CreateTime = entity.CreateTime;
            CustomerID = entity.CustomerID;
            DeviceID = entity.DeviceID;
            Flights = entity.Flights;
            FlightTime = entity.FlightTime;
            FieldName = entity.FieldName;
            FlywayPoints = entity.FlywayPoints;
            TaskLocation = entity.TaskLocation;
            TeamName = entity.Device.Team?.Name;
            TaskArea = entity.TaskArea;
            TaskAreaUnit = entity.TaskAreaUnit;
        }
    }
}