
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using MiSmart.Infrastructure.Validations;

namespace MiSmart.API.Commands
{
    public class AddingLogFileCommand
    {
        [AllowedExtensions(new String[] { ".bin" })]
        [Required]
        public IFormFile File { get; set; }
    }
    public class AddingPlanCommand
    {
        [AllowedExtensions(new String[] { ".plan" })]
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public Double? Latitude { get; set; }
        [Required]
        public Double? Longitude { get; set; }
        [Required]
        public Double? Area { get; set; }
        public Byte[] GetFileBytes()
        {
            Byte[] bytes = new Byte[] { };
            using (var ms = new MemoryStream())
            {
                this.File.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
    public class RetrievingPlansCommand
    {
        public String Search { get; set; }
        [Required]
        public Double? Latitude { get; set; }
        [Required]
        public Double? Longitude { get; set; }
        [Required]
        public Double? Range { get; set; }
    }
    public class RetrievingPlanFileCommand
    {
        [Required]
        public Int64? PlanID { get; set; }
    }
}