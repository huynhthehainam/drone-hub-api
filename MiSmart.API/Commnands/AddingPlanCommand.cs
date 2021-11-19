
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using MiSmart.Infrastructure.Validations;

namespace MiSmart.API.Commands
{
    public class AddingPlanCommand
    {
        [AllowedExtensions(new String[] { ".plan" })]
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public String Prefix { get; set; }
        [Required]
        public Double? Latitude { get; set; }
        [Required]
        public Double? Longitude { get; set; }
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
}