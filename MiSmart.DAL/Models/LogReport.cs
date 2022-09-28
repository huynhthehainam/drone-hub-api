using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;


namespace MiSmart.DAL.Models
{
    public class LogReport : EntityBase<Int64>
    {
        public LogReport() : base() { }
        public LogReport(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public Guid LogFileID {get; set;}
        private LogFile logFile;
        public LogFile LogFile {
            get => lazyLoader.Load(this, ref logFile);
            set => logFile = value;
        }
        public Guid UserUUID { get; set; }
        public String PilotDescription {get;set;}
        public String ReporterDescription {get;set;}
        public List<String> ImageUrls {get;set;} 
        public DateTime AccidentTime{get; set;}
        public DateTime UpdatedTime {get; set;} = DateTime.UtcNow;
        public String Suggest {get; set; }
        public String PilotName {get; set; }
        public String PartnerCompanyName {get; set; }
    }
}