

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class RemovingCustomerUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
    }
    public class RemovingExecutionCompanyUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
    }
    public class RemovingDeviceCommand
    {
        [Required]
        public Int64? DeviceID { get; set; }
    }
}