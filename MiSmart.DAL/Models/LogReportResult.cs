using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponsibleCompany
    {
        MiSmart,
        NoCompany,
        AnotherCompany,
    }
    public class LogReportResult : EntityBase<Int64>
    {
        public LogReportResult() : base() { }
        public LogReportResult(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public Guid LogFileID { get; set; }
        private LogFile? logFile;
        public LogFile? LogFile
        {
            get => lazyLoader.Load(this, ref logFile);
            set => logFile = value;
        }
        public String? Token { get; set; } = TokenHelper.GenerateToken(17);
        public List<String>? ImageUrls { get; set; }
        public String? Suggest { get; set; }
        public String? Conclusion { get; set; }
        public String? DetailedAnalysis { get; set; }
        public Guid AnalystUUID { get; set; }
        public Guid? ApproverUUID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
        private ExecutionCompany? executionCompany;
        public ExecutionCompany? ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        private ICollection<LogResultDetail>? logResultDetails;
        public ICollection<LogResultDetail>? LogResultDetails
        {
            get => lazyLoader.Load(this, ref logResultDetails);
            set => logResultDetails = value;
        }
        public ResponsibleCompany ResponsibleCompany { get; set; } = ResponsibleCompany.NoCompany;
        public String? ApproverName { get; set; }
        public String? AnalystName { get; set; }
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}