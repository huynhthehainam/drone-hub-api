using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class TelemetryRecordViewModel : IViewModel<TelemetryRecord>
    {
        public Guid ID { get; set; }
        public DateTime CreatedTime { get; set; }
        public CoordinateViewModel LocationPoint { get; set; }
        public Int64 GroupID { get; set; }

        public void LoadFrom(TelemetryRecord entity)
        {
            ID = entity.ID;
            CreatedTime = entity.CreatedTime;
            LocationPoint = new CoordinateViewModel(entity.LocationPoint.Coordinate);
            GroupID = entity.GroupID;
        }
    }
}