using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MiSmart.API.Commands
{
    public class UpdatingDeviceModelImageCommand
    {
        [Required]
        public IFormFile? File { get; set; }
    }
     public class UpdatingBatteryModelImageCommand
    {
        [Required]
        public IFormFile? File { get; set; }
    }
}