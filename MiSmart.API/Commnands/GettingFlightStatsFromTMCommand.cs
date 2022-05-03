

using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class GettingFlightStatsFromTMCommand
    {
        [Required]
        public String TMUserUID { get; set; }
    }
}