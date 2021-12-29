
using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallFlightStatViewModel : IViewModel<FlightStat>
    {
        public Guid ID { get; set; }
        public Double FlightDuration { get; set; }
        public DateTime CreatedTime { get; set; }
        // public Int32 CustomerID { get; set; }
        // public Int32 DeviceID { get; set; }
        public String AircraftName { get; set; }
        public Int32 Flights { get; set; }
        public DateTime FlightTime { get; set; }
        public String FieldName { get; set; }
        public String TaskLocation { get; set; }
        public String TeamName { get; set; }
        public Double TaskArea { get; set; }
        public String PilotName { get; set; }
        public void LoadFrom(FlightStat entity)
        {
            ID = entity.ID;
            FlightDuration = entity.FlightDuration;
            CreatedTime = entity.CreatedTime;
            // CustomerID = entity.CustomerID;
            // DeviceID = entity.DeviceID;
            PilotName = entity.PilotName;
            Flights = entity.Flights;
            FlightTime = entity.FlightTime;
            FieldName = entity.FieldName;
            TaskLocation = entity.TaskLocation;
            TeamName = entity.Device.Team?.Name;
            AircraftName = entity.Device.Name;
            TaskArea = entity.TaskArea;
        }
    }
}