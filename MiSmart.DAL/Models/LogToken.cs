
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
namespace MiSmart.DAL.Models
{
    public class LogToken : EntityBase<Int64>
    {
        public LogToken() : base()
        {
        }

        public LogToken(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Guid LogFileID { get; set; }
        private LogFile? logFile;
        public LogFile? LogFile
        {
            get => lazyLoader.Load(this, ref logFile);
            set => logFile = value;
        }
        public Guid UserUUID { get; set; }
        public String? Token { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public String? Username { get; set; }
    }
}