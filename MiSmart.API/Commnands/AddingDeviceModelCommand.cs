
using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AddingDeviceModelCommand
    {
        [Required]
        public String? Name { get; set; }
        [Required]
        public DeviceModelType Type {get;set; }
    }
}