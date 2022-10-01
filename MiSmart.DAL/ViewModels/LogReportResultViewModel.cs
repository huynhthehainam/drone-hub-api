using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogResultDetailViewModel
    {
        public Int64 PartErrorID { get; set; }
        public String Detail { get; set; }
        public StatusError Status { get; set; }
        public String Resolve { get; set; }
    }
    public class LogReportResultViewModel : IViewModel<LogReportResult>
    {
        public Guid LogFileID {get; set;}
        public List<String> ImageUrls{get; set;}
        public String Suggest {get; set;}
        public Guid AnalystUUID {get; set;}
        public Guid? ApproverUUID {get; set;}
        public List<LogResultDetailViewModel> ListErrors {get; set; }
        public String Conclusion {get; set; }
        public String DetailAnalysis {get; set; }
        public ResponsibleCompany ResponsibleCompany {get; set; }
        public void LoadFrom(LogReportResult entity)
        {
            LogFileID = entity.LogFileID;
            ImageUrls = entity.ImageUrls;
            Suggest = entity.Suggest;
            Conclusion = entity.Conclusion;
            DetailAnalysis = entity.DetailedAnalysis;
            AnalystUUID = entity.AnalystUUID;
            ApproverUUID = entity.ApproverUUID;
            ListErrors = new List<LogResultDetailViewModel>();
            foreach(var error in entity.LogResultDetails){
                LogResultDetailViewModel detail = new LogResultDetailViewModel{
                    Detail = error.Detail,
                    PartErrorID = error.PartErrorID,
                    Status = error.Status,
                    Resolve = error.Resolve,
                };
                ListErrors.Add(detail);
            }
            ResponsibleCompany = entity.ResponsibleCompany;
        }
    }
}