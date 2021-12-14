

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingDeviceCommand
    {
        [Required]
        public String Name { get; set; }
        public Int32? CustomerID { get; set; }
        [Required]
        public Int32? DeviceModelID { get; set; }
        public Int64? TeamID { get; set; }
    }
}