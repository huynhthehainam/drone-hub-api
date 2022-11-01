

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class GeneratingDeviceAccessTokenCommand
    {
        [Required]
        public String? DeviceToken { get; set; }
    }
}