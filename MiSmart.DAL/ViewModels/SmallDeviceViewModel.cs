
using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public Int64? TeamID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public DeviceStatus Status { get; set; }
        public Guid UUID { get; set; }
        public String TeamName { get; set; }
        public String DeviceModelName { get; set; }
        public Guid? LastGroupID { get; set; }
        public List<TelemetryRecordViewModel> LastGroupRecords { get; set; }

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
        }
    }
}