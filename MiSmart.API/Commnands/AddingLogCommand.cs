using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AddingLogReportCommand{
        public String PilotDescription {get; set; }
        public String ReporterDescription {get; set; }
        public String Suggest {get; set; }
        public DateTime AccidentTime { get; set; }
        public String PilotName {get; set; }
        public String PartnerCompanyName {get; set; }

    }
    public class AddingLogResultCommand{
        public List<AddingLogResultDetailCommand> ListErrors {get; set; }
        public String Suggest {get; set; }
        public String Conclusion {get; set; }
        public String DetailedAnalysis {get; set; }
        public Int32? ExecutionCompanyID {get; set;}
    }
    public class AddingLogResultDetailCommand{
        public Int32 PartID {get; set; }
        public StatusError Status {get; set; }
        public String Detail {get; set; }
        public String Resolve {get; set; } 
    }
    public class AddingLogResultFromMailCommand{
        public String Token {get; set; }
        public List<AddingLogResultDetailCommand> ListErrors {get; set; }
        public String Suggest {get; set; }
        public String Conclusion {get; set; }
        public String DetailedAnalysis {get; set; }
        public Int32? ExecutionCompanyID {get; set;}
    }
    // public class TestDroneLogCommand{
    //     public IFormFile File {get; set; }
    // }
    public class AddingLogReportFromEmailCommand{
        public String Token {get; set; }
        public String PilotDescription {get; set; }
        public String ReporterDescription {get; set; }
        public String Suggest {get; set; }
        public DateTime AccidentTime { get; set; }
        public String PilotName {get; set; }
        public String PartnerCompanyName {get; set; }
    }
    public class AddingLogErrorCommand{
        public Guid Id {get; set; }
        public String Error {get; set; }
    }
    public class AddingGetLogForEmailCommand{
        public String Token {get; set; }
    }
    public class AddingLogImageLinkCommand{
        [Required]
        public List<IFormFile> Files { get; set; }
    }
    public class AddingLogImageLinkFromEmailCommand{
        public String Token {get; set; }
        [Required]
        public List<IFormFile> Files { get; set; }
    }

}