using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class TelemetryRecordViewModel : IViewModel<TelemetryRecord>
    {
        public Guid ID { get; set; }
        public DateTime CreatedTime { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Int32 DeviceID { get; set; }

        public void LoadFrom(TelemetryRecord entity)
        {
            ID = entity.ID;
            CreatedTime = entity.CreatedTime;
            Latitude = entity.Latitude;
            Longitude = entity.Longitude;
            DeviceID = entity.DeviceID;
        }
    }
}