using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogFileViewModel : IViewModel<LogFile>
    {
        public Guid ID { get; set; }
        public String FileName { get; set; }
        public Int32 DeviceID { get; set; }
        public DateTime LoggingTime { get; set; }
        public void LoadFrom(LogFile entity)
        {
            ID = entity.ID;
            FileName = entity.FileName;
            DeviceID = entity.DeviceID;
            LoggingTime = entity.LoggingTime;
        }
    }
}