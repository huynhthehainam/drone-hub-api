

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MiSmart.Infrastructure.Validations;

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
        [AllowedExtensions(new String[] { ".jpg", ".png", ".mp3", ".wav", ".mid", ".aif", ".mp4", ".avi" })]
        public IFormFile File { get; set; }
    }
}