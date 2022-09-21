using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogReportResultViewModel : IViewModel<LogReportResult>
    {
        public Guid LogFileID {get; set;}
        public String[] ImageUrls{get; set;}
        public String Suggest {get; set;}
        public Guid AnalystUUID {get; set;}
        public Guid ApproverUUID {get; set;}
        public String ExecutionCompanyName {get; set; }
        public void LoadFrom(LogReportResult entity)
        {
            LogFileID = entity.LogFileID;
            ImageUrls = entity.ImageUrls;
            Suggest = entity.Suggest;
            AnalystUUID = entity.AnalystUUID;
            ApproverUUID = entity.ApproverUUID;
            ExecutionCompanyName = entity.ExecutionCompany.Name;
        }
    }
}