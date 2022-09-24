using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Validations;

namespace MiSmart.API.Commands
{
    public class AddingLogReportCommand{
        [AllowedExtensions(new[] {".jpg",".png","jpeg"})]
        [MaxFileSize(5 * 1024 * 1024)]
        public List<IFormFile> Files {get; set; } = new List<IFormFile>();
        public String PilotDescription {get; set; }
        public String ReporterDescription {get; set; }
        public DateTime AccidentTime { get; set; }
    }
    public class AddingLogDetailCommand{
        public Double FlightDuration { get; set; }
        public XYZ Vibe {get; set;}
        public Double PercentBattery { get; set; }
        public Double PercentFuel {get; set;}
        public EdgeData Edge {get; set;}
        public Double BatteryCellDeviation {get; set;}
        public Double Area {get; set;}
        public Double FlySpeed {get;set;}
        public Double Heigh {get; set;}
        public XYZ Accel {get;set;} 
    }
    public class AddingLogResultCommand{
        public List<LogResultDetail> ListErrors {get; set; }
        public String Suggest {get; set; }
        public String Conclusion {get; set; }
        public String DetailedAnalysis {get; set; }
        public Int32? ExecutionCompanyID {get; set;}
        
        [AllowedExtensions(new[] {".jpg",".png","jpeg"})]
        [MaxFileSize(5 * 1024 * 1024)]
        public List<IFormFile> Files {get; set; }
    }
    public class AddingLogResultFromMailCommand{
        public String Token {get; set; }
        public List<LogResultDetail> ListErrors {get; set; }
        public String Suggest {get; set; }
        public String Conclusion {get; set; }
        public String DetailedAnalysis {get; set; }
        public Int32? ExecutionCompanyID {get; set;}
        
        [AllowedExtensions(new[] {".jpg",".png","jpeg"})]
        [MaxFileSize(5 * 1024 * 1024)]
        public List<IFormFile> Files {get; set; }
    }
    // public class TestDroneLogCommand{
    //     public IFormFile File {get; set; }
    // }
    public class AddingLogReportFromEmailCommand{
        public String Token {get; set; }

        [AllowedExtensions(new[] {".jpg",".png","jpeg"})]
        [MaxFileSize(5 * 1024 * 1024)]
        public List<IFormFile> Files {get; set; } = new List<IFormFile>();
        public String PilotDescription {get; set; }
        public String ReporterDescription {get; set; }
        public DateTime AccidentTime { get; set; }
    }
    public class AddingLogErrorCommand{
        public String Error {get; set; }
    }
    public class AddingLogDetailForEmailCommand{
        public String Token {get; set; }
    }
}