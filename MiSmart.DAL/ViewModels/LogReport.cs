using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogReportViewModel : IViewModel<LogReport>
    {
        public Guid LogFileID {get; set;}
        public Guid UserUUID { get; set; }
        public String PilotDescription {get;set;}
        public String ReporterDescription {get;set;}
        public String[] ImageUrls {get;set;} 
        public DateTime AccidentTime{get; set;}
        public DateTime UpdatedTime {get; set;} = DateTime.UtcNow;
        public void LoadFrom(LogReport entity)
        {
            LogFileID = entity.LogFileID;
            UserUUID = entity.UserUUID;
            PilotDescription = entity.PilotDescription;
            ReporterDescription = entity.ReporterDescription;
            ImageUrls = entity.ImageUrls;
            AccidentTime = entity.AccidentTime;
            UpdatedTime = entity.UpdatedTime;
        }
    }
}