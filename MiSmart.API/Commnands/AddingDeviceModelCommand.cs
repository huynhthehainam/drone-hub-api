
using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingDeviceModelCommand
    {
        [Required]
        public String Name { get; set; }
    }
}