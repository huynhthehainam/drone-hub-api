using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SecondLogReportViewModel : IViewModel<SecondLogReport>
    {
        public Guid LogFileID {get; set;}
        public Guid UserUUID { get; set; }
        public String? PilotDescription {get;set;}
        public String? ReporterDescription {get;set;}
        public List<String>? ImageUrls {get;set;} 
        public DateTime AccidentTime{get; set;}
        public DateTime UpdatedTime {get; set;} = DateTime.UtcNow;
        public String? Suggest {get; set; }
        public String? PilotName {get; set; }
        public String? PartnerCompanyName {get; set; }
        public String? Token {get; set; }
        public String? Username { get; set; }
        public void LoadFrom(SecondLogReport entity)
        {
            LogFileID = entity.LogFileID;
            UserUUID = entity.UserUUID;
            PilotDescription = entity.PilotDescription;
            ReporterDescription = entity.ReporterDescription;
            ImageUrls = entity.ImageUrls;
            AccidentTime = entity.AccidentTime;
            UpdatedTime = entity.UpdatedTime;
            Suggest = entity.Suggest;
            PilotName = entity.PilotName;
            PartnerCompanyName = entity.PartnerCompanyName;
            Token = entity.Token;
            Username = entity.Username;
        }
    }
}