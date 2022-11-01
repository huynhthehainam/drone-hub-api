using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AddingLogReportCommand
    {
        public String? PilotDescription { get; set; }
        public String? ReporterDescription { get; set; }
        public String? Suggest { get; set; }
        public DateTime AccidentTime { get; set; } = DateTime.UtcNow;
        public String? PilotName { get; set; }
        public String? PartnerCompanyName { get; set; }
    }
    public class AddingSecondLogReportCommand
    {
        public String? PilotDescription { get; set; }
        public String? ReporterDescription { get; set; }
        public String? Suggest { get; set; }
        public DateTime AccidentTime { get; set; } = DateTime.UtcNow;
        public String? PilotName { get; set; }
        public String? PartnerCompanyName { get; set; }
        public List<String> ImageUrls { get; set; } = new List<String>();
    }
    public class AddingLogResultCommand
    {
        public List<AddingLogResultDetailCommand> ListErrors { get; set; } = new List<AddingLogResultDetailCommand>();
        public String? Suggest { get; set; }
        public String? Conclusion { get; set; }
        public String? DetailedAnalysis { get; set; }
        public ResponsibleCompany ResponsibleCompany { get; set; }
    }
    public class AddingLogResultDetailCommand
    {
        [Required]
        public Int32? PartID { get; set; }
        [Required]
        public StatusError Status { get; set; }
        public String? Detail { get; set; }
        public String? Resolve { get; set; }
    }
    public class AddingLogResultFromMailCommand
    {
        [Required]
        public String? Token { get; set; }
        public List<AddingLogResultDetailCommand>? ListErrors { get; set; }
        public String? Suggest { get; set; }
        public String? Conclusion { get; set; }
        public String? DetailedAnalysis { get; set; }
        public ResponsibleCompany ResponsibleCompany { get; set; }
    }
    public class AddingLogReportFromEmailCommand
    {
        [Required]
        public String? Token { get; set; }
        public String? PilotDescription { get; set; }
        public String? ReporterDescription { get; set; }
        public String? Suggest { get; set; }
        public DateTime AccidentTime { get; set; }
        public String? PilotName { get; set; }
        public String? PartnerCompanyName { get; set; }
        public String? Username { get; set; }
    }
    public class AddingLogErrorCommand
    {
        public String? Message { get; set; }
    }
    public class AddingGetLogForEmailCommand
    {
        [Required]
        public String? Token { get; set; }
    }
    public class AddingLogImageLinkCommand
    {
        [Required]
        public List<IFormFile>? Files { get; set; }
    }
    public class AddingLogImageLinkFromEmailCommand
    {
        [Required]
        public String ?Token { get; set; }
        [Required]
        public List<IFormFile>? Files { get; set; }
    }
    public class AddingLogErrorForEmailCommand
    {
        public String? Message { get; set; }
        public String? Token { get; set; }
    }
    public class AddingSecondLogReportFromEmailCommand
    {
        [Required]
        public String? Token { get; set; }
        public String? PilotDescription { get; set; }
        public String? ReporterDescription { get; set; }
        public String? Suggest { get; set; }
        public DateTime AccidentTime { get; set; }
        public String? PilotName { get; set; }
        public String? PartnerCompanyName { get; set; }
        public String? Username { get; set; }
        public List<String> ImageUrls { get; set; } = new List<String>();
    }

}