




using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssingingDeviceCommand
    {
        [Required]
        public Int32? TeamID { get; set; }
    }
}