using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public enum StatusError{
        Good,
        Follow,
        Bad,
    }

    public class LogResultDetail : EntityBase<Int64>
    {
        public LogResultDetail() : base() { }
        public LogResultDetail(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public Int32 PartErrorID {get; set; }
        private Part partError;
        public Part PartError {
            get => lazyLoader.Load(this, ref partError);
            set => partError = value;
        }
        public StatusError Status {get; set; }
        public String Detail {get; set; }
        public String Resolve {get; set; }
        public Int64 LogReportResultID {get; set; }
        private LogReportResult logReportResult;
        public LogReportResult LogReportResult{
            get => lazyLoader.Load(this, ref logReportResult);
            set => logReportResult = value;
        }
    }
}