
using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class BatteryLogViewModel : IViewModel<BatteryLog>
    {
        public Double CellMaximumVoltage { get; set; }
        public String CellMaximumVoltageUnit { get; set; }
        public Double CellMinimumVoltage { get; set; }
        public String CellMinimumVoltageUnit { get; set; }
        public DateTime CreatedTime { get; set; }
        public Double Current { get; set; }
        public String CurrentUnit { get; set; }
        public Int32 CycleCount { get; set; }
        public Guid ID { get; set; }
        public Double PercentRemaining { get; set; }
        public Double Temperature { get; set; }
        public String TemperatureUnit { get; set; }

        public void LoadFrom(BatteryLog entity)
        {
            CellMaximumVoltage = entity.CellMaximumVoltage;
            CellMaximumVoltageUnit = entity.CellMaximumVoltageUnit;
            CellMinimumVoltage = entity.CellMinimumVoltage;
            CellMinimumVoltageUnit = entity.CellMinimumVoltageUnit;
            CreatedTime = entity.CreatedTime;
            Current = entity.Current;
            CurrentUnit = entity.CurrentUnit;
            CycleCount = entity.CycleCount;
            ID = entity.ID;
            PercentRemaining = entity.PercentRemaining;
            Temperature = entity.Temperature;
            TemperatureUnit = entity.TemperatureUnit;

        }
    }
    public class BatteryGroupLogViewModel : IViewModel<BatteryGroupLog>
    {
        public Int32 BatteryID { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<BatteryLogViewModel> Logs { get; set; }
        public Guid ID { get; set; }

        public void LoadFrom(BatteryGroupLog entity)
        {
            BatteryID = entity.BatteryID;
            CreatedTime = entity.CreatedTime;
            Logs = entity.Logs.OrderBy(ww => ww.CreatedTime).Select(l => ViewModelHelpers.ConvertToViewModel<BatteryLog, BatteryLogViewModel>(l)).ToList();
            ID = entity.ID;
        }
    }
    public class SmallDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public Int64? TeamID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public DeviceStatus Status { get; set; }
        public Guid UUID { get; set; }
        public String TeamName { get; set; }
        public String ExecutionCompanyName { get; set; }
        public String DeviceModelName { get; set; }
        public Guid? LastGroupID { get; set; }
        public List<TelemetryRecordViewModel> LastGroupRecords { get; set; }
        public List<Guid> LastBatteryGroupIDs;

        public String CustomerName { get; set; }
        public List<BatteryGroupLogViewModel> BatteryGroupLogs { get; set; }
        public DateTime? LastOnline { get; set; }

        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            TeamID = entity.TeamID;
            DeviceModelName = entity.DeviceModel.Name;
            CustomerID = entity.CustomerID;
            Name = entity.Name;
            Status = entity.Status;
            UUID = entity.UUID;
            TeamName = entity.Team?.Name;
            LastGroupID = entity.LastGroupID;
            LastGroupRecords = entity.LastGroup?.Records.OrderBy(ww => ww.CreatedTime).Select(ww => ViewModelHelpers.ConvertToViewModel<TelemetryRecord, TelemetryRecordViewModel>(ww)).ToList();
            ExecutionCompanyName = entity.ExecutionCompany?.Name;
            LastBatteryGroupIDs = entity.LastBatterGroupLogs;
            CustomerName = entity.Customer.Name;
            LastOnline = entity.LastOnline;
        }
    }
    public class LargeDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public Int64? TeamID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public DeviceStatus Status { get; set; }
        public Guid UUID { get; set; }
        public String TeamName { get; set; }
        public String ExecutionCompanyName { get; set; }
        public String DeviceModelName { get; set; }
        public Guid? LastGroupID { get; set; }
        public List<TelemetryRecordViewModel> LastGroupRecords { get; set; }
        public List<Guid> LastBatteryGroupIDs;
        public List<BatteryGroupLogViewModel> BatteryGroupLogs { get; set; }
        public String Token { get; set; }
        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            TeamID = entity.TeamID;
            DeviceModelName = entity.DeviceModel.Name;
            CustomerID = entity.CustomerID;
            Name = entity.Name;
            Status = entity.Status;
            UUID = entity.UUID;
            TeamName = entity.Team?.Name;
            LastGroupID = entity.LastGroupID;
            LastGroupRecords = entity.LastGroup?.Records.OrderBy(ww => ww.CreatedTime).Select(ww => ViewModelHelpers.ConvertToViewModel<TelemetryRecord, TelemetryRecordViewModel>(ww)).ToList();
            ExecutionCompanyName = entity.ExecutionCompany?.Name;
            LastBatteryGroupIDs = entity.LastBatterGroupLogs;
            Token = entity.Token;
        }
    }
}