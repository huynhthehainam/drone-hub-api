using System;
using System.Collections.Generic;
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
        public List<String> ImageUrls {get;set;} 
        public DateTime AccidentTime{get; set;}
        public DateTime UpdatedTime {get; set;} = DateTime.UtcNow;
        public String Suggest {get; set; }
        public void LoadFrom(LogReport entity)
        {
            LogFileID = entity.LogFileID;
            UserUUID = entity.UserUUID;
            PilotDescription = entity.PilotDescription;
            ReporterDescription = entity.ReporterDescription;
            ImageUrls = entity.ImageUrls;
            AccidentTime = entity.AccidentTime;
            UpdatedTime = entity.UpdatedTime;
            Suggest = entity.Suggest;
        }
    }
}