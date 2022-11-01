using System;
using System.Collections.Generic;
using System.Text.Json;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogFileViewModel : IViewModel<LogFile>
    {
        public Guid ID { get; set; }
        public String? FileName { get; set; }
        public Int32 DeviceID { get; set; }
        public String? DeviceName { get; set; }
        public DateTime LoggingTime { get; set; }
        public DroneStatus DroneStatus { get; set; }
        public LogStatus Status { get; set; }
        public List<String>? Errors { get; set; }
        public String? ExecutionCompanyName { get; set; }
        public JsonDocument? Detail { get; set; }
        public Boolean isAnalyzed { get; set; }
        public String? Location { get; set; }
        public void LoadFrom(LogFile entity)
        {
            ID = entity.ID;
            FileName = entity.FileName;
            DeviceID = entity.DeviceID;
            DeviceName = entity.Device is null ? null : entity.Device.Name;
            LoggingTime = entity.LoggingTime;
            DroneStatus = entity.DroneStatus;
            Status = entity.Status;
            isAnalyzed = entity.IsAnalyzed;
            Location = entity.LogDetail?.Location;
            Errors = new List<String>();
            if (entity.LogReportResult is not null)
            {
                if (entity.LogReportResult.LogResultDetails is not null)
                {
                    foreach (LogResultDetail item in entity.LogReportResult.LogResultDetails)
                    {
                        if (item.Status == StatusError.Bad)
                        {
                            if (item.PartError is not null)
                            {
                                if (item.PartError.Name is not null)
                                    Errors.Add(item.PartError.Name);
                            }
                        }
                    }

                }

            }
        }
    }
}