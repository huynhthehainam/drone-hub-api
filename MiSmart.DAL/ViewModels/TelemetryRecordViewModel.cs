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
        public Double Direction { get; set; }
        public Object AdditionalInformation { get; set; }
        public Int64 GroupID { get; set; }

        public void LoadFrom(TelemetryRecord entity)
        {
            ID = entity.ID;
            CreatedTime = entity.CreatedTime;
            LocationPoint = new CoordinateViewModel(entity.LocationPoint.Coordinate);
            Direction = entity.Direction;
            AdditionalInformation = entity.AdditionalInformation;
            GroupID = entity.GroupID;
        }
    }
}