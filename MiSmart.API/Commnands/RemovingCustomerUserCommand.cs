

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class RemovingCustomerUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }
    }
    public class RemovingDeviceCommand
    {
        [Required]
        public Int64? DeviceID { get; set; }
    }
}