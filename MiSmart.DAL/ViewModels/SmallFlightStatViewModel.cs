
using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SuperSmallFlightStatViewModel : IViewModel<FlightStat>
    {
        public Guid ID { get; set; }
        public Int32 Flights { get; set; }
        public Double FlightDuration { get; set; }
        public Double TaskArea { get; set; }
        public String TaskLocation { get; set; }
        public DateTime FlightTime { get; set; }

        public void LoadFrom(FlightStat entity)
        {
            ID = entity.ID;
            Flights = entity.Flights;
            FlightDuration = entity.FlightDuration;
            TaskArea = entity.TaskArea;
        }
    }
    public class SmallFlightStatViewModel : IViewModel<FlightStat>
    {
        public Guid ID { get; set; }
        public Double FlightDuration { get; set; }
        public DateTime CreatedTime { get; set; }
        public String DeviceModelName { get; set; }
        public String AircraftName { get; set; }
        public Int32 Flights { get; set; }
        public DateTime FlightTime { get; set; }
        public String FieldName { get; set; }
        public String TaskLocation { get; set; }
        public String TeamName { get; set; }
        public Double TaskArea { get; set; }
        public String PilotName { get; set; }
        public Int64? TeamID { get; set; }
        public String ExecutionCompanyName { get; set; }
        public String CustomerName { get; set; }
        public Double Cost { get; set; }
        public String TMUserUID { get; set; }
        public TMUser TMUser { get; set; }
        public List<Medicine> Medicines { get; set; }
        public void LoadFrom(FlightStat entity)
        {
            ID = entity.ID;
            FlightDuration = entity.FlightDuration;
            CreatedTime = entity.CreatedTime;
            PilotName = entity.PilotName;
            Flights = entity.Flights;
            FlightTime = entity.FlightTime;
            FieldName = entity.FieldName;
            TaskLocation = entity.TaskLocation;
            TeamName = entity.Team?.Name;
            AircraftName = entity.Device.Name;
            TaskArea = entity.TaskArea;
            DeviceModelName = entity.Device.DeviceModel.Name;
            ExecutionCompanyName = entity.ExecutionCompany?.Name;
            CustomerName = entity.Customer.Name;
            Cost = entity.Cost;
            TeamID = entity.TeamID;
            TMUserUID = entity.TMUserUID;
            TMUser = entity.TMUser;
            Medicines = entity.Medicines;
        }
    }
}