
using System;
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
        public CoordinateViewModel LastPoint { get; set; }


        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            TeamID = entity.TeamID;
            CustomerID = entity.CustomerID;
            Name = entity.Name;
            Status = entity.Status;
            UUID = entity.UUID;
            TeamName = entity.Team?.Name;
            LastPoint = entity.LastPoint is not null ? new CoordinateViewModel(entity.LastPoint.Coordinate) : null;
        }
    }
}