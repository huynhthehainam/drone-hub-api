using System;
using System.Text.Json;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogFileViewModel : IViewModel<LogFile>
    {
        public Guid ID { get; set; }
        public String FileName { get; set; }
        public Int32 DeviceID { get; set; }
        public String DeviceName {get;set;}
        public DateTime LoggingTime { get; set; }
        public DroneStatus DroneStatus { get; set; }
        public LogStatus Status {get; set;}
        public String[] Errors {get; set;}
        public String ExecutionCompanyName {get; set;}
        public JsonDocument Detail {get; set;}
        public Boolean isAnalyzed {get; set; }
        public String Location {get; set; }
        public void LoadFrom(LogFile entity)
        {
            ID = entity.ID;
            FileName = entity.FileName;
            DeviceID = entity.DeviceID;
            DeviceName = entity.Device.Name;
            LoggingTime = entity.LoggingTime;
            DroneStatus = entity.DroneStatus;
            Status = entity.Status;
            Errors = entity.Errors;
            isAnalyzed = entity.isAnalyzed;
            Location = entity?.LogDetail?.Location;
        }
    }
}