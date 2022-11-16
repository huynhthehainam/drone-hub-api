

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingDeviceCommand
    {
        [Required]
        public String? Name { get; set; }
        [Required]
        public Int32? DeviceModelID { get; set; }
    }
    public class GetDeviceFromTMCommand
    {
        [Required]
        public String? EncryptedUUID { get; set; }
    }
}