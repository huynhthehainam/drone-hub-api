

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MiSmart.API.Commands
{
    public class AddingMaintenanceReportCommand
    {
        [Required]
        public String Reason { get; set; }
        public DateTime? ActualReportCreatedTime { get; set; }
    }
    public class AddingMaintenanceReportAttachmentCommand
    {
        [Required]
        public List<IFormFile> Files { get; set; }
    }
}