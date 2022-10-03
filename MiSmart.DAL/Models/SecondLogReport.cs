using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;

namespace MiSmart.DAL.Models
{
    public class SecondLogReport : EntityBase<Int64>
    {
        public SecondLogReport() : base() { }
        public SecondLogReport(ILazyLoader lazyLoader) : base(lazyLoader) { }
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
        public String Token { get; set; } = TokenHelper.GenerateToken(17);
        public String Username { get; set; }
    }
}