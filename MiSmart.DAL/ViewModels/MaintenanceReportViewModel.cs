
using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class MaintenanceReportViewModel : IViewModel<MaintenanceReport>
    {
        public Int32 ID { get; set; }
        public List<String> Attachments { get; set; }
        public Guid UserUUID { get; set; }
        public String Reason { get; set; }
        public void LoadFrom(MaintenanceReport entity)
        {
            ID = entity.ID;
            Attachments = entity.AttachmentLinks;
            UserUUID = entity.UserUUID;
            Reason = entity.Reason;
        }
    }
}