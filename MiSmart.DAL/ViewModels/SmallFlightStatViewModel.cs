
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        public String? TaskLocation { get; set; }
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
        public String? DeviceModelName { get; set; }
        public String? AircraftName { get; set; }
        public Int32 Flights { get; set; }
        public DateTime FlightTime { get; set; }
        public String? FieldName { get; set; }
        public String? TaskLocation { get; set; }
        public String? TeamName { get; set; }
        public Double TaskArea { get; set; }
        public String? PilotName { get; set; }
        public Int64? TeamID { get; set; }
        public String? ExecutionCompanyName { get; set; }
        public String? CustomerName { get; set; }
        public Double Cost { get; set; }
        public String? TMUserUID { get; set; }
        public String? TMPlantID { get; set; }
        public String? TMFieldID { get; set; }
        public JsonDocument? TMUser { get; set; }
        public JsonDocument? Medicines { get; set; }
        public CoordinateViewModel? FirstPoint { get; set; }
        public Boolean IsBingLocation { get; set; }
        public FlightStatStatus? Status { get; set; }
        public List<CoordinateViewModel>? Boundary { get; set; }

        public List<FlightStatReportRecordViewModel>? ReportRecords { get; set; }
        public void LoadFrom(FlightStat entity)
        {
            IsBingLocation = entity.IsBingLocation;
            ID = entity.ID;
            FlightDuration = entity.FlightDuration;
            CreatedTime = entity.CreatedTime;
            PilotName = entity.PilotName;
            Flights = entity.Flights;
            FlightTime = entity.FlightTime;
            FieldName = entity.FieldName;
            TaskLocation = entity.TaskLocation;
            TeamName = entity.Team?.Name;
            AircraftName = entity.Device is null ? null : entity.Device.Name;
            TaskArea = entity.TaskArea;
            DeviceModelName = (entity.Device is null || entity.Device.DeviceModel is null) ? null : entity.Device.DeviceModel.Name;
            ExecutionCompanyName = entity.ExecutionCompany?.Name;
            CustomerName = entity.Customer is null ? null : entity.Customer.Name;
            Cost = entity.Cost;
            TeamID = entity.TeamID;
            TMUserUID = entity.TMUserUUID;
            TMPlantID = entity.TMPlantID;
            TMFieldID = entity.TMFieldID;
            TMUser = entity.TMUser;
            Medicines = entity.Medicines;
            Status = entity.Status;
            if (entity.FlywayPoints != null && entity.FlywayPoints.Count > 0)
            {
                FirstPoint = new CoordinateViewModel(entity.FlywayPoints?.Coordinates.FirstOrDefault() ?? new NetTopologySuite.Geometries.Coordinate(0, 0));
            }
            ReportRecords = entity.FlightStatReportRecords is null ? null : entity.FlightStatReportRecords.Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStatReportRecord, FlightStatReportRecordViewModel>(ww)).ToList();
            if (entity.Boundary is not null)
            {
                Boundary = entity.Boundary.Coordinates.Select(ww => new CoordinateViewModel(ww)).ToList();
            }
        }
    }
}