
using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LargeFlightStatViewModel : IViewModel<FlightStat>
    {
        public Guid ID { get; set; }
        public Double FlightDuration { get; set; }
        public DateTime CreatedTime { get; set; }
        public Int32 CustomerID { get; set; }
        public Int32 DeviceID { get; set; }
        public Int32 Flights { get; set; }
        public DateTime FlightTime { get; set; }
        public String? FieldName { get; set; }
        public List<CoordinateViewModel>? FlywayPoints { get; set; }
        public List<Int32>? SprayedIndexes { get; set; }
        public String? TaskLocation { get; set; }
        public String? TeamName { get; set; }
        public Double TaskArea { get; set; }
        public void LoadFrom(FlightStat entity)
        {
            ID = entity.ID;
            FlightDuration = entity.FlightDuration;
            CreatedTime = entity.CreatedTime;
            CustomerID = entity.CustomerID;
            DeviceID = entity.DeviceID;
            Flights = entity.Flights;
            FlightTime = entity.FlightTime;
            FieldName = entity.FieldName;
            FlywayPoints = entity.FlywayPoints?.Coordinates.Select(ww => new CoordinateViewModel(ww)).ToList();
            SprayedIndexes = entity.SprayedIndexes;
            TaskLocation = entity.TaskLocation;
            TeamName = entity.Device?.Team?.Name;
            TaskArea = entity.TaskArea;
        }
    }
}